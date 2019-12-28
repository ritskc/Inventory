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
    this.gridColumns.push( new DataColumn({ headerText: "Email", value: "email", customStyling: 'column-width-100' }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closing Date", value: "closingDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closed", value: "closed", isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Edit', actionStyle: ClassConstants.Primary, event: 'editPurchaseOrder', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deletePurchaseOrder', icon: 'fa fa-trash' })
    ] }) );
  }

  initializeGridColumnsForDetails() {
    this.gridColumns = [];
    this.gridColumns.push( new DataColumn({ headerText: "Supplier", value: "supplierName", sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "PO Number", value: "poNo", isLink: true, sortable: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Date", value: "poDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Due Date", value: "dueDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Closing Date", value: "closingDate", sortable: true, isDate: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Part Code", value: "partCode"}) );
    this.gridColumns.push( new DataColumn({ headerText: "Part Desc", value: "partDescription", minWidth: true }) );
    this.gridColumns.push( new DataColumn({ headerText: "Qty", value: "qty"}) );
    this.gridColumns.push( new DataColumn({ headerText: "Price", value: "unitPrice"}) );
    this.gridColumns.push( new DataColumn({ headerText: "Total", value: "total"}) );
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
                viewModel.closingDate = order.closingDate;
                viewModel.closed = detail.isClosed;
                viewModel.partCode = detail.part.code;
                viewModel.partDescription = detail.part.description;
                viewModel.qty = detail.qty;
                viewModel.unitPrice = detail.unitPrice;
                viewModel.total = detail.qty * detail.unitPrice;
                this.purchaseOrderViewModels.push(viewModel);
              })
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
              this.purchaseOrderViewModels.push(viewModel);
            }
          })
        }, (error) => this.toastr.errorToastr(error.error),
        () => this.loaderService.hide());
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
          this.toastr.errorToastr('This PO cannot be edited since this is already closed!!');
          return;
        }
        this.router.navigateByUrl(`orders/detail/supplier/${data.supplierId}/edit/${data.id}`);
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
  total: number;
}