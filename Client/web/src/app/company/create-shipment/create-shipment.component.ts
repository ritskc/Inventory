import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { CustomerService } from '../../admin/customer/customer.service';
import { Customer, ShippingInfo } from '../../models/customer.model';
import { PartsService } from '../../admin/parts/parts.service';
import { Part } from '../../models/part.model';
import { PurchaseOrder } from '../../models/purchase-order';
import { Shipment, PackingSlipDetail } from '../../models/shipment.model';
import { DataColumn } from '../../models/dataColumn.model';
import { ShipmentService } from '../shipment.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-create-shipment',
  templateUrl: './create-shipment.component.html',
  styleUrls: ['./create-shipment.component.scss']
})
export class CreateShipmentComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private customers: Customer[] = [];
  private parts: Part[] = [];
  private customerAssociatedParts: Part[] = [];
  private customerPurchaseOrders: PurchaseOrder[] = [];
  private shipment: Shipment = new Shipment();
  private shipments: Shipment[] = [];
  private columnsForPartsGrid: DataColumn[] = [];
  private columnsForOrderGrid: DataColumn[] = [];
  private shipmentsViewModel: ShipmentsViewModel[] = [];

  private selectedCustomer: Customer = new Customer();

  private blankOrder: boolean = false;
  private orderId: number = 0;
  private partCode: number = 0;
  private quantity: number = 0;
  private inBasket: boolean = false;
  private boxes: number = 0;
  private packagingSlipCreated: Subject<string> = new Subject<string>();

  constructor(private companyservice: CompanyService, private customerService: CustomerService, private partsService: PartsService,
              private shipmentService: ShipmentService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyservice.getCurrentlyLoggedInCompanyId();
    this.shipment = new Shipment();
    this.shipment.CompanyId = this.currentlyLoggedInCompany;
    this.loadAllCustomers();
    this.loadAllParts();
  }

  loadAllCustomers() {
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe(
          (customers) => {
            let dummyCustomer = new Customer();
            dummyCustomer.id = -1; dummyCustomer.name = 'Select Customer'; this.customers.push(dummyCustomer);
            customers.forEach((customer) => {
              this.customers.push(customer);
            })
          },
          (error) => console.log(error),
          () => { console.log('Create Shipment -> Customers Loaded'); }
        );
  }

  loadAllParts() {
    this.partsService
      .getAllParts(this.currentlyLoggedInCompany)
      .subscribe(parts => {
        this.parts = parts;
      });
  }

  customerSelected(event) {
    this.customerAssociatedParts = [];
    this.customerPurchaseOrders = [];
    this.columnsForPartsGrid = [];
    this.selectedCustomer = this.customers.find(c => c.id == event.target.value);
    this.shipment.customerId = this.selectedCustomer.id;

    this.customerService.getAllPurchaseOrders(this.currentlyLoggedInCompany)
        .subscribe((orders) => {
          orders.forEach((order) => {
            if (order.customerId == this.selectedCustomer.id)
              this.customerPurchaseOrders.push(order);
          });
        }, (error) => console.log(error),
        () => console.log('Orders loaded'));

    this.parts.forEach((part) => {
      part.partCustomerAssignments.forEach((customer) => {
        if (customer.customerId == this.selectedCustomer.id)
          this.customerAssociatedParts.push(part);
      });
    });

    this.getAllShipmentsForSelectedCustomer();
    this.createColumnsForShipmentGrid();
  }

  getAllShipmentsForSelectedCustomer() {
    this.shipments = [];
    this.shipmentService.getAllShipments(this.currentlyLoggedInCompany)
        .subscribe(
          (shipments) => { 
            shipments.filter(s => s.customerId == this.selectedCustomer.id).forEach((shipment) => {
              this.shipments.push(shipment);
            });
          },
          (error) => { console.log(error); },
          () => { this.transformShipmentListToViewModel(); }
        );
  }

  addPart() {
    if(this.shipment.packingSlipDetails.length == 0) {
      this.createColumnsForPartsAddition();
    }

    var packagingSlipDetail = new PackingSlipDetail();
    packagingSlipDetail.isBlankOrder = this.blankOrder;
    packagingSlipDetail.orderId = this.orderId;
    packagingSlipDetail.partId = this.partCode;
    packagingSlipDetail.partDescription = this.parts.find(p => p.id == this.partCode).description;
    packagingSlipDetail.orderNo = this.customerPurchaseOrders.find(o => o.id == this.orderId).poNo;
    packagingSlipDetail.qty = this.quantity;
    packagingSlipDetail.boxes = this.boxes;
    packagingSlipDetail.inBasket = this.inBasket;
    this.shipment.packingSlipDetails.push(packagingSlipDetail);
  }

  createColumnsForPartsAddition() {
    this.columnsForPartsGrid = [];
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Blank Order", value: "isBlankOrder" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Order Id", value: "orderNo" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Part", value: "partDescription" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Quantity", value: "qty" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "In Basket", value: "inBasket" }) );
  }

  createColumnsForShipmentGrid() {
    this.columnsForOrderGrid = [];
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Pack Slip No", value: "packagingSlipNumber" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Date", value: "shippingDate" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "PO Number", value: "poNo" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Part Code", value: "partCode" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Quantity", value: "quantity" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Boxes", value: "boxes" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Creates", value: "crates" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Shipment Via", value: "shipVia" }) );
  }

  createShipment() {
    this.shipmentService.createShipment(this.shipment)
        .subscribe(
          (result) => {
            this.toastr.successToastr('Shipment Created Successfully!!');
            this.packagingSlipCreated.next(`http://renovate.yellow-chips.com/ReportViewer/PackingSlip.aspx?id=${result}`);
            this.shipment = new Shipment();
          },
          (error) => { this.toastr.errorToastr('Error while creating shipment'); console.log(error); }
        );
  }

  transformShipmentListToViewModel() {
    this.shipmentsViewModel = [];
    this.shipments.forEach((shipment) => {
      shipment.packingSlipDetails.forEach((detail) => {
        var shipmentViewModel = new ShipmentsViewModel();
        shipmentViewModel.packagingSlipNumber = shipment.packingSlipNo;
        shipmentViewModel.shippingDate = shipment.shippingDate;
        shipmentViewModel.poNo = detail.orderNo;
        shipmentViewModel.partCode = this.parts.find(p => p.id == detail.partId).code;
        shipmentViewModel.quantity = detail.qty;
        shipmentViewModel.boxes = detail.boxes;
        shipmentViewModel.creates = shipment.crates;
        shipmentViewModel.shipVia = shipment.shipVia;
        this.shipmentsViewModel.push(shipmentViewModel);
      });
    });
  }
}

export class ShipmentsViewModel {
  packagingSlipNumber: string = '';
  shippingDate: string = '';
  poNo: string = '';
  partCode: string = '';
  quantity: number = 0;
  boxes: number = 0;
  creates: number = 0;
  shipVia: string = '';
}