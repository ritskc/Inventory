import { Component, OnInit } from '@angular/core';
import { Customer } from '../../../models/customer.model';
import { PurchaseOrder } from '../../../models/purchase-order';
import { DataColumn } from '../../../models/dataColumn.model';
import { CustomerService } from '../customer.service';
import { CompanyService } from '../../../company/company.service';
import { Router, ActivatedRoute } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';

@Component({
  selector: 'app-purchase-orders',
  templateUrl: './purchase-orders.component.html',
  styleUrls: ['./purchase-orders.component.scss']
})
export class PurchaseOrdersComponent implements OnInit {

  private currentlyLoggedInCompanyid: number = 0;
  private customers: Customer[] = [];
  private purchaseOrders: CustomerPurchaseOrderViewModel[] = [];
  private gridColumns: DataColumn[] = [];
  private customerId: number;

  constructor(private customerService: CustomerService, private companyService: CompanyService, private router: Router, 
    private loaderService: httpLoaderService, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyid = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
    this.initializeGridColumns();
  }

  loadAllCustomers() {
    this.customerService.getAllCustomers(this.currentlyLoggedInCompanyid)
      .subscribe((customers) => {
        this.customers = customers;
        
        this.customerId = this.activatedRoute.snapshot.params.id;
        this.loadAllPurchaseOrders();        
      });
  }

  initializeGridColumns() {
    this.gridColumns.push( new DataColumn({ headerText: "Number", value: "poNo", isLink: true, sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Part Code", value: "partCode", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Part Description", value: "partDescription", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Open Qty", value: "openQuantity", sortable: true }) );
  }

  loadAllPurchaseOrders() {
    this.loaderService.show();
    this.customerService.getAllPurchaseOrders(this.currentlyLoggedInCompanyid)
        .subscribe((purchaseOrders) => {
          this.populatePurchaseOrderViewModel(purchaseOrders);
          this.loaderService.hide();
        }, (error) => {
          this.loaderService.hide();
        });
  }

  populatePurchaseOrderViewModel(purchaseOrders: PurchaseOrder[]) {
    this.purchaseOrders = [];
    var customerOrders = this.customerId > 0? purchaseOrders.filter(p => p.customerId == this.customerId): purchaseOrders;
    customerOrders.forEach((order) => {
      order.orderDetails.forEach((detail) => {
        var customerPurchaseOrderViewModel = new CustomerPurchaseOrderViewModel();
        customerPurchaseOrderViewModel.poNo = order.poNo;
        customerPurchaseOrderViewModel.poDate = order.poDate;
        customerPurchaseOrderViewModel.dueDate = order.closingDate;
        customerPurchaseOrderViewModel.partCode = detail.part.code;
        customerPurchaseOrderViewModel.partDescription = detail.part.description;
        customerPurchaseOrderViewModel.openQuantity = detail.part.openingQty;
        this.purchaseOrders.push(customerPurchaseOrderViewModel);
      });
    });
  }

  addCustomerOrder() {
    this.router.navigateByUrl(`orders/detail/customer/${this.customerId}`);
  }
}

class CustomerPurchaseOrderViewModel {
  poNo: string = '';
  poDate: string = '';
  dueDate: string = '';
  partCode: string = '';
  partDescription: string = '';
  openQuantity: number = 0;
}