import { Component, OnInit } from '@angular/core';
import { Customer } from '../../../models/customer.model';
import { PurchaseOrder } from '../../../models/purchase-order';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { CustomerService } from '../customer.service';
import { CompanyService } from '../../../company/company.service';
import { Router, ActivatedRoute } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { map, mergeMap } from 'rxjs/operators';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../../common/constants';
import { AppConfigurations } from '../../../config/app.config';

@Component({
  selector: 'app-purchase-orders',
  templateUrl: './purchase-orders.component.html',
  styleUrls: ['./purchase-orders.component.scss']
})
export class PurchaseOrdersComponent implements OnInit {

  private currentlyLoggedInCompanyid: number = 0;
  private customers: Customer[] = [];
  private pos: PurchaseOrder[] = [];
  private purchaseOrders: CustomerPurchaseOrderViewModel[] = [];
  private gridColumns: DataColumn[] = [];
  private customerId: number;
  private showFullDetails: boolean = false;
  private showOpenOrders: boolean = false;

  constructor(private customerService: CustomerService, private companyService: CompanyService, private router: Router, 
    private loaderService: httpLoaderService, private activatedRoute: ActivatedRoute, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyid = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
    this.initializeGridColumns();
  }

  loadAllCustomers() {
    this.loaderService.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompanyid).pipe(
      map(customers => {
        this.customers = customers;
        this.customerId = this.activatedRoute.snapshot.params.id;
        return customers;
      }),
      mergeMap(customers => this.getAllPurchaseOrders())
    ).subscribe(purchaseOrders => {
      this.pos = purchaseOrders;
      this.populatePurchaseOrderViewModel(purchaseOrders);
      this.customerId = -1;
    }, (error) => this.toastr.errorToastr(error.error)
    ,() => this.loaderService.hide());
  }

  initializeGridColumns() {
    this.gridColumns = [];
    this.gridColumns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "PO Number", value: "poNo", isLink: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "PO Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "isClosed", isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Order', actionStyle: ClassConstants.Primary, event: 'downloadDocument', icon: 'fa fa-download' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteOrder', icon: 'fa fa-trash' })
    ] }) );  
  }

  initializeGridForDetails() {
    this.gridColumns = [];
    this.gridColumns.push( new DataColumn({ headerText: "PO Number", value: "poNo", isLink: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "PO Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Line", value: "lineNo", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Code", value: "partCode", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Description", value: "partDescription", sortable: true, customStyling: 'column-width-100' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Order", value: "orderedQty", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Open", value: "openQuantity", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Shipped", value: "shippedQty", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", customStyling: 'right' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Status", value: "status" }) );
    this.gridColumns.push( new DataColumn({ headerText: "Force Close", value: "isForceClosed", isBoolean: true, isDisabled: true, customStyling: 'center' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Note", value: "note" }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "isClosed", isBoolean: true, customStyling: 'center', isDisabled: true }) );
  }

  loadAllPurchaseOrders() {
    this.loaderService.show();
    this.getAllPurchaseOrders()
        .subscribe((purchaseOrders) => {
          this.pos = purchaseOrders;
          this.populatePurchaseOrderViewModel(purchaseOrders);
        }, (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());
  }

  populatePurchaseOrderViewModel(purchaseOrders: PurchaseOrder[]) {
    this.loaderService.show();
    this.purchaseOrders = [];
    var customerOrders = this.customerId > 0? purchaseOrders.filter(p => p.customerId == this.customerId): purchaseOrders;
    customerOrders = this.showOpenOrders ? customerOrders.filter(p => p.isClosed === false): customerOrders;

    customerOrders.forEach((order) => {
      order.customerName = this.customers.find(c => c.id == order.customerId).name;
      if (this.showFullDetails) {
        order.orderDetails.forEach((detail) => {
          var customerPurchaseOrderViewModel = new CustomerPurchaseOrderViewModel();
          customerPurchaseOrderViewModel.customerId = order.customerId;
          customerPurchaseOrderViewModel.id = order.id;
          customerPurchaseOrderViewModel.poNo = order.poNo;
          customerPurchaseOrderViewModel.poDate = order.poDate;
          customerPurchaseOrderViewModel.dueDate = detail.dueDate;
          customerPurchaseOrderViewModel.partCode = detail.part.code;
          customerPurchaseOrderViewModel.partDescription = detail.part.description;
          customerPurchaseOrderViewModel.openQuantity = detail.qty - detail.shippedQty;
          customerPurchaseOrderViewModel.customerName = order.customerName;
          customerPurchaseOrderViewModel.orderedQty = detail.qty;
          customerPurchaseOrderViewModel.isClosed = detail.isClosed;
          customerPurchaseOrderViewModel.shippedQty = detail.shippedQty;
          customerPurchaseOrderViewModel.lineNo = detail.lineNumber;
          customerPurchaseOrderViewModel.unitPrice = detail.unitPrice;
          customerPurchaseOrderViewModel.note = detail.note;
          customerPurchaseOrderViewModel.status = detail.isClosed? 'Close': 'Open';
          customerPurchaseOrderViewModel.isForceClosed = detail.isForceClosed;
          this.purchaseOrders.push(customerPurchaseOrderViewModel);
        });
      } else {
        var customerPurchaseOrderViewModel = new CustomerPurchaseOrderViewModel();
          customerPurchaseOrderViewModel.customerId = order.customerId;
          customerPurchaseOrderViewModel.id = order.id;
          customerPurchaseOrderViewModel.poNo = order.poNo;
          customerPurchaseOrderViewModel.poDate = order.poDate;
          customerPurchaseOrderViewModel.dueDate = order.dueDate;
          customerPurchaseOrderViewModel.customerName = order.customerName;
          customerPurchaseOrderViewModel.isClosed = order.isClosed;
          this.purchaseOrders.push(customerPurchaseOrderViewModel);
      }
    });
    this.loaderService.hide();
  }

  addCustomerOrder() {
    if (this.customerId < 1) {
      this.toastr.warningToastr('Please select the customer to proceed');
      return;
    }
    this.router.navigateByUrl(`orders/detail/customer/${this.customerId}/create/0`);
  }

  rowSelected(row: any) {
    this.router.navigateByUrl(`orders/detail/customer/${row.customerId}/edit/${row.id}`);
  }

  private getAllPurchaseOrders() {
    return this.customerService.getAllPurchaseOrders(this.currentlyLoggedInCompanyid);
  }

  showFullOrderDetails(event) {
    console.log(event);
    if (this.showFullDetails)
      this.initializeGridForDetails();
    else
      this.initializeGridColumns();
    
    this.populatePurchaseOrderViewModel(this.pos);
  }

  showOnlyOpenOrdersEvent() {
    this.populatePurchaseOrderViewModel(this.pos);
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'downloadDocument':
        var configuration = new AppConfigurations();
        window.open(`${configuration.fileApiUri}/customerorder/${data.id}`);
        break;
      case 'deleteOrder':
        var result = confirm('Are you sure you want to remove this customer order?');
        if (result) {
          this.loaderService.show();
          this.customerService.deletePurchaseOrder(data.id)
              .subscribe(() => {
                this.toastr.successToastr('Customer Order removed successfully!');
                this.loaderService.hide();
              },
              (error) => {
                this.toastr.errorToastr(error.error);
                this.loaderService.hide();
              },
              () => {
                this.loadAllPurchaseOrders();
              });
        }
        break;
    }
  }
}

class CustomerPurchaseOrderViewModel {
  poNo: string = '';
  poDate: string = '';
  dueDate: string = '';
  partCode: string = '';
  partDescription: string = '';
  openQuantity: number = 0;
  customerId: number = 0;
  id: number = 0
  customerName: string = '';
  orderedQty: number = 0;
  isClosed: boolean = false;
  shippedQty: number = 0;
  lineNo: number = 0;
  unitPrice: number = 0;
  note: string = '';
  status: string = '';
  isForceClosed: boolean;
}