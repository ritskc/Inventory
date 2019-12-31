import { Component, OnInit } from '@angular/core';
import { Customer } from '../../../models/customer.model';
import { PurchaseOrder } from '../../../models/purchase-order';
import { DataColumn } from '../../../models/dataColumn.model';
import { CustomerService } from '../customer.service';
import { CompanyService } from '../../../company/company.service';
import { Router, ActivatedRoute } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { map, mergeMap } from 'rxjs/operators';
import { ToastrManager } from 'ng6-toastr-notifications';

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
    this.gridColumns.push( new DataColumn({ headerText: "Number", value: "poNo", isLink: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "isClosed", isBoolean: true, customStyling: 'center', isDisabled: true }) );
  }

  initializeGridForDetails() {
    this.gridColumns = [];
    this.gridColumns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Number", value: "poNo", isLink: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Code", value: "partCode", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Description", value: "partDescription", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Open", value: "openQuantity" }) );
    this.gridColumns.push( new DataColumn({ headerText: "Order", value: "orderedQty" }) );
    this.gridColumns.push( new DataColumn({ headerText: "Shipped", value: "shippedQty" }) );
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
          customerPurchaseOrderViewModel.dueDate = order.closingDate;
          customerPurchaseOrderViewModel.partCode = detail.part.code;
          customerPurchaseOrderViewModel.partDescription = detail.part.description;
          customerPurchaseOrderViewModel.openQuantity = detail.part.openingQty;
          customerPurchaseOrderViewModel.customerName = order.customerName;
          customerPurchaseOrderViewModel.orderedQty = detail.qty;
          customerPurchaseOrderViewModel.isClosed = detail.isClosed;
          customerPurchaseOrderViewModel.shippedQty = detail.shippedQty;
          this.purchaseOrders.push(customerPurchaseOrderViewModel);
        });
      } else {
        var customerPurchaseOrderViewModel = new CustomerPurchaseOrderViewModel();
          customerPurchaseOrderViewModel.customerId = order.customerId;
          customerPurchaseOrderViewModel.id = order.id;
          customerPurchaseOrderViewModel.poNo = order.poNo;
          customerPurchaseOrderViewModel.poDate = order.poDate;
          customerPurchaseOrderViewModel.dueDate = order.closingDate;
          customerPurchaseOrderViewModel.customerName = order.customerName;
          customerPurchaseOrderViewModel.isClosed = order.isClosed;
          this.purchaseOrders.push(customerPurchaseOrderViewModel);
      }
    });
    this.loaderService.hide();
  }

  addCustomerOrder() {
    if (this.customerId < 1) {
      this.toastr.errorToastr('Please select the customer to proceed');
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
}