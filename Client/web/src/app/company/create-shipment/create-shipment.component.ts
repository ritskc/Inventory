import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { CustomerService } from '../../admin/customer/customer.service';
import { Customer, ShippingInfo } from '../../models/customer.model';
import { PartsService } from '../../admin/parts/parts.service';
import { Part } from '../../models/part.model';
import { PurchaseOrder } from '../../models/purchase-order';
import { Shipment, PackingSlipDetail } from '../../models/shipment.model';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ShipmentService } from '../shipment.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { AppConfigurations } from '../../config/app.config';
import * as DateHelper from '../../common/helpers/dateHelper';
import { UserAction } from '../../models/enum/userAction';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ClassConstants } from '../../common/constants';

@Component({
  selector: 'app-create-shipment',
  templateUrl: './create-shipment.component.html',
  styleUrls: ['./create-shipment.component.scss']
})
export class CreateShipmentComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private selectedCustomerId: number = 0;
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
  private appConfig: AppConfigurations;
  private previousPackingSlipNo: string;
  private partQuantityInHand: number = 0;
  private partOpenQuantity: number = 0;
  private OrderNo: string = '';
  private unitPrice: number = 0;

  constructor(private companyservice: CompanyService, private customerService: CustomerService, private partsService: PartsService, private httpLoaderService: httpLoaderService, 
              private shipmentService: ShipmentService, private toastr: ToastrManager, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.appConfig = new AppConfigurations();
    this.selectedCustomerId = this.activatedRoute.snapshot.params.id;
    this.currentlyLoggedInCompany = this.companyservice.getCurrentlyLoggedInCompanyId();
    this.shipment = new Shipment();
    this.shipment.CompanyId = this.currentlyLoggedInCompany;
    this.loadAllParts();
    this.loadAllCustomers();
    this.shipment.shippingDate = DateHelper.getToday();
    this.shipmentService.getLatestShipment(this.currentlyLoggedInCompany, DateHelper.getToday()).subscribe(data => {
        this.previousPackingSlipNo = data ? data.entityNo : 'Data Unavailable';
    });
  }

  loadAllCustomers() {
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe(
          (customers) => {
            let dummyCustomer = new Customer();
            dummyCustomer.id = -1; dummyCustomer.name = 'Select Customer'; this.customers.push(dummyCustomer);
            customers.forEach((customer) => {
              this.customers.push(customer);
            });
            if (this.selectedCustomerId) {
              this.customerSelected(null);
            }
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
    this.selectedCustomer = this.customers.find(c => c.id == (event ? event.target.value: this.selectedCustomerId));
    this.shipment.customerId = this.selectedCustomer.id;

    this.loadAllPartsForCustomer();
    this.loadAllOrdersForCustomer();
    this.createColumnsForShipmentGrid();
    this.getAllShipmentsForSelectedCustomer();
  }

  loadAllPartsForCustomer() {
    this.customerAssociatedParts = [];
    this.parts.forEach((part) => {
      part.partCustomerAssignments.forEach((customer) => {
        if (customer.customerId == this.selectedCustomer.id)
          this.customerAssociatedParts.push(part);
      });
    });
    this.partCode = -1;
  }

  loadAllOrdersForCustomer() {
    this.customerService.getAllPurchaseOrders(this.currentlyLoggedInCompany)
      .subscribe((orders) => {
        this.customerPurchaseOrders = [];
        orders.forEach((order) => {
          if (order.customerId == this.selectedCustomer.id && !order.isClosed)
            this.customerPurchaseOrders.push(order);
        });
      }, (error) => console.log(error),
      () => this.orderId = -1 );
  }

  orderSelected() {
    this.customerAssociatedParts = [];
    this.customerPurchaseOrders.filter(c => c.id == this.orderId)[0].orderDetails
        .filter(o => !o.isClosed)
        .forEach(orderDetail => {
      this.customerAssociatedParts.push(orderDetail.part);
    });
    this.partCode = -1;
  }

  blankOrderChecked() {
    if (this.blankOrder) {
      this.loadAllPartsForCustomer();
    }
    this.orderId = -1;
    this.resetPartDetail();
    this.partQuantityInHand = 0;
    this.partOpenQuantity = 0;
}

  partSelected() {
    this.displayPartQuantityStatus();
    this.resetPartDetail();
  }

  shippedQuantityEntered() {
    this.displayPartQuantityStatus();
  }

  displayPartQuantityStatus() {
    if (this.blankOrder) {
      var selectedPart = this.customerAssociatedParts.find(p => p.id == this.partCode);
      this.partQuantityInHand = selectedPart.qtyInHand + selectedPart.openingQty;
      this.partOpenQuantity = 0;
      this.unitPrice = selectedPart.partCustomerAssignments.find(c => c.customerId == this.selectedCustomerId).rate;
    }
    else {
      var selectedOrder = this.customerPurchaseOrders.find(o => o.id == this.orderId);
      var selectedPart = this.customerAssociatedParts.find(p => p.id == this.partCode);
      var partDetail = selectedOrder.orderDetails.find(p => p.partId == selectedPart.id);
      this.partQuantityInHand = selectedPart.qtyInHand;
      this.partOpenQuantity = partDetail.qty - partDetail.shippedQty;
      this.unitPrice = selectedPart.partCustomerAssignments.find(c => c.customerId == this.selectedCustomerId).rate;
    }
  }

  getAllShipmentsForSelectedCustomer() {
    this.httpLoaderService.show();
    this.shipments = [];
    this.shipmentService.getAllShipments(this.currentlyLoggedInCompany)
        .subscribe(
          (shipments) => { 
            shipments.filter(s => s.customerId == this.selectedCustomer.id).forEach((shipment) => {
              this.shipments.push(shipment);
            });
          },
          (error) => { console.log(error); },
          () => { 
            this.httpLoaderService.hide();
            this.transformShipmentListToViewModel();
            this.loadShipmentForEditMode();
          }
        );
  }

  loadShipmentForEditMode() {
    if (this.activatedRoute.snapshot.params.action == UserAction.Edit) {
      this.httpLoaderService.show();
      this.shipmentService.getAShipment(this.currentlyLoggedInCompany, this.activatedRoute.snapshot.params.shipmentId)
          .subscribe((shipment) => {
              this.loadAllParts();
              this.createColumnsForPartsAddition();
              this.previousPackingSlipNo = this.shipment.packingSlipNo;
              shipment.packingSlipDetails.forEach(item => {
                item.partDescription = item.partDetail.description;
              });
              this.shipment = shipment;
              this.shipment.shippingDate = DateHelper.formatDate(new Date(shipment.shippingDate));
            },
            (error) => this.toastr.errorToastr(error.error),
            () => this.httpLoaderService.hide());
      return;
    }
  }

  addPart() {
    if(this.shipment.packingSlipDetails.length == 0) {
      this.createColumnsForPartsAddition();
    }

    if (this.quantity < 1) {
      this.toastr.errorToastr('Quantity should be more than 0(zero)');
      return;
    }

    if ((this.partQuantityInHand - this.quantity) < 0) {
      var response = confirm('Your inventory quantity is going to be negative. Are you sure to continue?');
      if (!response) return;
    }

    if (this.boxes <= 0) {
      var response = confirm('Number of boxes is 0. Are you sure to continue?');
      if (!response) return;
    }

    var packagingSlipDetail = new PackingSlipDetail();
    packagingSlipDetail.isBlankOrder = this.blankOrder;
    packagingSlipDetail.orderId = this.blankOrder ? 0: this.orderId;
    if (!this.blankOrder) {
      var selectedOrder = this.customerPurchaseOrders.find(o => o.id == this.orderId);
      var partInTheOrder = selectedOrder.orderDetails.find(p => p.partId == this.partCode);
      packagingSlipDetail.orderDetailId = partInTheOrder.id;
    } else {
      packagingSlipDetail.orderDetailId = 0;
    }
    packagingSlipDetail.partId = this.partCode;
    packagingSlipDetail.partDescription = this.parts.find(p => p.id == this.partCode).description;
    packagingSlipDetail.orderNo = this.blankOrder? this.OrderNo : this.customerPurchaseOrders.find(o => o.id == this.orderId).poNo;
    packagingSlipDetail.qty = this.quantity;
    packagingSlipDetail.boxes = this.boxes;
    packagingSlipDetail.inBasket = this.inBasket;
    packagingSlipDetail.excessQty = 0;
    packagingSlipDetail.unitPrice = this.unitPrice;
    this.shipment.packingSlipDetails.push(packagingSlipDetail);

    this.OrderNo = '';
    this.orderId = -1;
    this.partCode = -1;
    this.partQuantityInHand = 0;
    this.partOpenQuantity = 0;
    this.unitPrice = 0;
    this.resetPartDetail();
    this.resetSerialNumber();
  }

  resetSerialNumber() {
    var serialNo: number = 0;
    this.shipment.packingSlipDetails.forEach(item => item.srNo = ++serialNo);
  }

  createColumnsForPartsAddition() {
    this.columnsForPartsGrid = [];
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Sr No", value: "srNo", customStyling: 'center' }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Blank Order", value: "isBlankOrder", isDisabled: true, isBoolean: true, customStyling: 'center' }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Order Id", value: "orderNo" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Part", value: "partDescription" }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Quantity", value: "qty", isEditable: true, customStyling: 'right' }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", isEditable: true, customStyling: 'right' }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "In Basket", value: "inBasket", isBoolean: true, isDisabled: true, customStyling: 'center' }) );
    this.columnsForPartsGrid.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Danger, event: 'removePart', icon: 'fa fa-trash' })
    ] }) );  }

  createColumnsForShipmentGrid() {
    this.columnsForOrderGrid = [];
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Pack Slip No", value: "packagingSlipNumber" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Date", value: "shippingDate", isDate: true }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "PO Number", value: "poNo" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Part Code", value: "partCode" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Quantity", value: "quantity" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Boxes", value: "boxes" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Creates", value: "crates" }) );
    this.columnsForOrderGrid.push( new DataColumn({ headerText: "Shipment Via", value: "shipVia" }) );
  }

  createShipment() {
    if (!this.validateShipment())
      return;

    this.httpLoaderService.show();
    this.shipmentService.createShipment(this.shipment)
        .subscribe(
          (result) => {
            this.toastr.successToastr('Shipment Created Successfully!!');
            this.packagingSlipCreated.next(`${this.appConfig.reportsUri}/PackingSlip.aspx?id=${result}`);
            this.loadAllOrdersForCustomer();
            this.shipment = new Shipment();
          },
          (error) => { this.toastr.errorToastr('Error while creating shipment'); console.log(error); },
          () => {
            this.getAllShipmentsForSelectedCustomer();
            this.httpLoaderService.hide();
          }
        );
  }

  resetPartDetail() {
    this.quantity = 0;
    this.boxes = 0;
  }

  transformShipmentListToViewModel() {
    this.shipmentsViewModel = [];
    this.shipments.forEach((shipment) => {
      shipment.packingSlipDetails.forEach((detail) => {
        var shipmentViewModel = new ShipmentsViewModel();
        shipmentViewModel.packagingSlipNumber = shipment.packingSlipNo;
        shipmentViewModel.shippingDate = shipment.shippingDate;
        shipmentViewModel.poNo = detail.orderNo;
        shipmentViewModel.partCode = detail.partDetail? detail.partDetail.code: this.parts.find(p => p.id == detail.partId).code;
        shipmentViewModel.quantity = detail.qty;
        shipmentViewModel.boxes = detail.boxes;
        shipmentViewModel.creates = shipment.crates;
        shipmentViewModel.shipVia = shipment.shipVia;
        this.shipmentsViewModel.push(shipmentViewModel);
      });
    });
  }

  validateShipment(): boolean {
    if (!this.shipment.shipVia) {
      this.toastr.errorToastr('Please enter valid shipment via detail');
      return false;
    }
    if (this.shipment.customerId < 1) {
      this.toastr.errorToastr('Please select the customer');
      return false;
    }
    if (this.shipment.shipmentInfoId < 1) {
      this.toastr.errorToastr('Please select valid shipment address');
      return false;
    }
    if (!this.shipment.packingSlipDetails) {
      this.toastr.errorToastr('Add at least one part detail to create shipment');
      return false;
    }

    return true;
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'removePart':
        var index = this.shipment.packingSlipDetails.findIndex(d => d.srNo == data.srNo);
        this.shipment.packingSlipDetails.splice(index, 1);
        break;
    }
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