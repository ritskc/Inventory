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
  private columnsForPartsGrid: DataColumn[] = [];

  private selectedCustomer: Customer = new Customer();

  private blankOrder: boolean = false;
  private orderId: number = 0;
  private partCode: number = 0;
  private quantity: number = 0;
  private inBasket: boolean = false;

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
    this.shipment.CustomerId = this.selectedCustomer.id;

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
  }

  addPart() {
    if(this.shipment.PackingSlipDetails.length == 0) {
      this.createColumnsForPartsAddition();
    }

    var packagingSlipDetail = new PackingSlipDetail();
    packagingSlipDetail.IsBlankOrder = this.blankOrder;
    packagingSlipDetail.OrderId = this.orderId;
    packagingSlipDetail.PartId = this.partCode;
    packagingSlipDetail.PartDescription = this.parts.find(p => p.id == this.partCode).description;
    packagingSlipDetail.OrderNumber = this.customerPurchaseOrders.find(o => o.id == this.orderId).poNo;
    packagingSlipDetail.Qty = this.quantity;
    packagingSlipDetail.InBasket = this.inBasket;
    this.shipment.PackingSlipDetails.push(packagingSlipDetail);
  }

  createColumnsForPartsAddition() {
    this.columnsForPartsGrid = [];
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Blank Order", value: "IsBlankOrder" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Order Id", value: "OrderNumber" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Part", value: "PartDescription" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Quantity", value: "Qty" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "In Basket", value: "InBasket" }) );
  }

  createShipment() {
    this.shipmentService.createShipment(this.shipment)
        .subscribe(
          (result) => { this.toastr.successToastr('Shipment Created Successfully!!') },
          (error) => { this.toastr.errorToastr('Error while creating shipment'); console.log(error); }
        );
  }
}