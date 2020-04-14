import { Component, OnInit } from '@angular/core';
import { SupplierService } from '../supplier.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Supplier } from '../../../models/supplier.model';
import { UserAction } from '../../../models/enum/userAction';
import { FormControl, FormGroup, FormBuilder } from '@angular/forms';
import { PurchaseOrder } from '../../../models/purchase-order';
import { ClassConstants } from '../../../common/constants';
import { ToastrManager } from 'ng6-toastr-notifications';
import * as DateHelper from '../../../common/helpers/dateHelper';
import { Subject } from 'rxjs';
import { AppConfigurations } from '../../../config/app.config';

@Component({
  selector: 'app-purchase-order-list',
  templateUrl: './purchase-order-list.component.html',
  styleUrls: ['./purchase-order-list.component.scss']
})
export class PurchaseOrderListComponent implements OnInit {

  private purchaseOrders: PurchaseOrder[] = [];
  private purchaseOrderViewModels: SupplierPurchaseOrderViewModel[] = [];
  private gridColumns: DataColumn[] = [];
  private currentlyLoggedInCompanyid: number = 0;
  private supplierId: number = -1;
  private suppliers: Supplier[];
  private showFullDetails: boolean = false;
  private showOnlyOpenOrders: boolean = false;
  private showLateOrders: boolean = false;
  private showAcknowledgedOrders: boolean = false;
  printDocument: Subject<string> = new Subject<string>();

  constructor(private service: SupplierService, private companyService: CompanyService, private formBuilder: FormBuilder,
              private loaderService: httpLoaderService, private router: Router, private activatedRoute: ActivatedRoute, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyid = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllSuppliers();
    this.initializeGridColumns();
  }

  initializeGridColumns() {
    this.gridColumns = [];
    this.gridColumns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "PO Number", value: "poNo", isLink: true, sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Email", value: "email", minWidth: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closing", value: "closingDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Ack", value: "isAcknowledged", isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "closed", isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'PO', actionStyle: ClassConstants.Primary, event: 'printPurchaseOrder', icon: 'fa fa-print' }),
      new DataColumnAction({ actionText: 'Edit', actionStyle: ClassConstants.Primary, event: 'editPurchaseOrder', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deletePurchaseOrder', icon: 'fa fa-trash' })
    ] }));
  }

  initializeGridColumnsForDetails() {
    this.gridColumns = [];
    this.gridColumns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "PO Number", value: "poNo", isLink: true, sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Part Code", value: "partCode", columnName: 'PartCode' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Part Desc", value: "partDescription", columnName: 'PartDescription', minWidth: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Qty", value: "qty", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Open Qty", value: "openQty", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Price", value: "unitPrice", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Total", value: "total", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Rcvd", value: "receivedQty", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Transit", value: "inTransitQty", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Force Closed", value: "forceClosed", isBoolean: true, isDisabled: true, customStyling: 'center' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "closed", isBoolean: true, isDisabled: true, customStyling: 'center' }) );
  }

  loadAllSuppliers(){
    this.loaderService.show();
    this.service.getAllSuppliers(this.currentlyLoggedInCompanyid)
        .subscribe((suppliers) => {
          this.suppliers = suppliers;
          this.loadAllPurchaseOrders();
        }, (error) => {
          this.toastr.errorToastr(error.error);
        }, () => this.loaderService.hide());
  }

  loadAllPurchaseOrders() {
    this.loaderService.show();
    this.service.getPurchaseOrders(this.currentlyLoggedInCompanyid)
        .subscribe((purchaseOrders) => {
          this.purchaseOrderViewModels = [];
          this.purchaseOrders = this.supplierId > 0? purchaseOrders.filter(p => p.supplierId == this.supplierId): purchaseOrders;
          this.purchaseOrders = this.showOnlyOpenOrders ? this.purchaseOrders.filter(p => p.isClosed === false): this.purchaseOrders;
          this.purchaseOrders = this.showAcknowledgedOrders ? this.purchaseOrders.filter(p => p.isAcknowledged == true): this.purchaseOrders;

          this.purchaseOrders.forEach(order => {
            if (this.showFullDetails) {
              order.poDetails.forEach(detail => {
                var viewModel = new SupplierPurchaseOrderViewModel();
                viewModel.id = order.id;
                viewModel.supplierId = order.supplierId;
                viewModel.supplierName = this.suppliers.find(s => s.id === order.supplierId).name;
                viewModel.poNo = order.poNo;
                viewModel.email = order.emailIds;
                viewModel.poDate = order.poDate;
                viewModel.dueDate = order.dueDate;
                viewModel.lateOrder = viewModel.dueDate && DateHelper.convertToDateTime(viewModel.dueDate) < new Date()? true: false;
                viewModel.closingDate = order.closingDate;
                viewModel.closed = detail.isClosed;
                viewModel.partCode = detail.part.code;
                viewModel.partDescription = detail.part.description;
                viewModel.qty = detail.qty;
                viewModel.unitPrice = detail.unitPrice;
                viewModel.total = (viewModel.qty * viewModel.unitPrice).toFixed(2);
                viewModel.acknowledgedQty = detail.ackQty;
                viewModel.receivedQty = parseInt(detail.receivedQty);
                viewModel.inTransitQty = detail.inTransitQty;
                viewModel.forceClosed = detail.isForceClosed;
                viewModel.openQty = detail.qty - (viewModel.inTransitQty + viewModel.receivedQty);
                this.purchaseOrderViewModels.push(viewModel);
              });
            } else {
              var viewModel = new SupplierPurchaseOrderViewModel();
                viewModel.id = order.id;
                viewModel.supplierId = order.supplierId;
                viewModel.supplierName = this.suppliers.find(s => s.id === order.supplierId).name;
                viewModel.poNo = order.poNo;
                viewModel.email = order.emailIds;
                viewModel.poDate = order.poDate;
                viewModel.dueDate = order.dueDate;
                viewModel.closingDate = order.closingDate;
                viewModel.closed = order.isClosed;
                viewModel.isAcknowledged = order.isAcknowledged;
                viewModel.lateOrder = viewModel.dueDate && DateHelper.convertToDateTime(viewModel.dueDate) < new Date()? true: false;
                this.purchaseOrderViewModels.push(viewModel);
            }
          })
        }, (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());

        this.purchaseOrderViewModels = this.showLateOrders? this.purchaseOrderViewModels.filter(p => p.lateOrder === true): this.purchaseOrderViewModels;
  }

  rowSelected(event) {
    this.redirectToPurchaseOrderDetails(event);
  }

  redirectToPurchaseOrderDetails(row: any){
    this.router.navigateByUrl(`orders/detail/supplier/${row.supplierId}/edit/${row.id}`);
  }

  supplierSelected() {
    this.loadAllPurchaseOrders();
  }

  addPurchaseOrder() {
    if (this.supplierId > 0) {
      this.router.navigateByUrl(`orders/detail/supplier/${this.supplierId}/create/0`);
      return;
    }
    alert('Please select a supplier id to proceed');
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'deletePurchaseOrder':
        var response = confirm('Are you sure you want to remove this purchase order?');
        if (response) {
          this.service.deletePurchaseOrder(data.id)
          .subscribe(
            () => this.toastr.successToastr('Purchase order removed successfully!'),
            (error) => this.toastr.errorToastr(error.error),
            () => this.ngOnInit()
          );
        }
        break;
      case 'editPurchaseOrder':
        if (data.isClosed) {
          this.toastr.warningToastr('This PO cannot be edited since this is already closed!!');
          return;
        }
        this.router.navigateByUrl(`orders/detail/supplier/${data.supplierId}/edit/${data.id}`);
        break;
      case 'printPurchaseOrder':
        var appConfig = new AppConfigurations();
        this.printDocument.next(`${appConfig.reportsUri}/supplierpo.aspx?id=${data.id}`);
        break;
    }
  }

  showFullOrderDetails(event) {
    if (this.showFullDetails) {
      this.initializeGridColumnsForDetails();
    } else {
      this.initializeGridColumns();
    }
    this.loadAllPurchaseOrders();
  }

  showOnlyOpenOrdersEvent(event) {
    this.loadAllPurchaseOrders();
  }

  showLateOrdersEvent(event) {
    this.loadAllPurchaseOrders();
  }

  showOnlyAcknowledgedOrdersEvent(event) {
    this.loadAllPurchaseOrders();
  }
}

class SupplierPurchaseOrderViewModel {
  id: number;
  supplierId: number;
  supplierName: string;
  poNo: string;
  email: string;
  poDate: string;
  dueDate: string;
  closingDate: string;
  closed: boolean;
  partCode: string;
  partDescription: string;
  qty: number;
  unitPrice: number;
  total: string;
  lateOrder: boolean = false;
  inTransitQty: number;
  receivedQty: number;
  acknowledgedQty: number;
  isAcknowledged: boolean;
  forceClosed: boolean;
  openQty: number;
}