import { Component, OnInit, Output } from '@angular/core';
import { SupplierService } from '../supplier.service';
import { CompanyService } from '../../../company/company.service';
import { Supplier } from '../../../models/supplier.model';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { UserAction } from '../../../models/enum/userAction';
import { Router } from '@angular/router';
import { ClassConstants } from '../../../common/constants';
import { EventEmitter } from 'protractor';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-supplier-list',
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss']
})
export class SupplierListComponent implements OnInit {

  suppliers: Supplier[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;

  constructor(private supplierService: SupplierService, private companyService: CompanyService,
              private loaderService: httpLoaderService, private router: Router, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllSuppliers();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Name", value: "name", sortable: true, customStyling: 'column-width-150' }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "phoneNo", sortable: true, customStyling: 'column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "address", customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "City", value: "city", customStyling: 'column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "State", value: "state", customStyling: 'column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Fax", value: "faxNo", customStyling: 'column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editSupplier' }),
      new DataColumnAction({ actionText: 'PO', actionStyle: ClassConstants.Primary, event: 'managePurchaseOrder' }),
      new DataColumnAction({ actionText: 'Invoice', actionStyle: ClassConstants.Primary, event: 'manageInvoice' }),
      new DataColumnAction({ actionText: 'Receive', actionStyle: ClassConstants.Primary, event: 'manageReceiveInvoice' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteSupplier' })
    ] }) );
  }

  getAllSuppliers() {
    this.loaderService.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompanyId)
      .subscribe(
        (suppliers) => {
          this.suppliers = suppliers;
          this.loaderService.hide();
        },
        (error) => {
          console.log(error);
          this.loaderService.hide();
        }
      )
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/suppliers/detail/${ UserAction.Edit }/${row.id}`);
  }

  addSupplier() {
    this.router.navigateByUrl(`/suppliers/detail/${ UserAction.Add }/#`);
  }

  redirectToSupplierPurchaseOrder(supplier) {
    this.router.navigateByUrl(`/suppliers/purchase-order/${ supplier.id }/${ UserAction.Details }`);
  }

  redirectToSupplierInvoice(supplier) {
    this.router.navigateByUrl(`/invoice/upload/${ supplier.id }/0`);
  }

  redirectToReceiveInvoice(supplier) {
    this.router.navigateByUrl(`/invoice/receive/${ supplier.id }`);
  }

  deleteSupplier(row) {
    if (confirm('Are you sure you want to delete this supplier?')) {
      this.supplierService.delete(row.id)
          .subscribe(
            () => this.router.navigateByUrl('/suppliers'),
            (error) => this.toastr.errorToastr(error.error)
          );
    }
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editSupplier':
        this.rowSelected(data);
        break;
      case 'managePurchaseOrder':
        this.redirectToSupplierPurchaseOrder(data);
        break;
      case 'manageInvoice':
        this.redirectToSupplierInvoice(data);
        break;
      case 'manageReceiveInvoice':
        this.redirectToReceiveInvoice(data);
        break;
      case 'deleteSupplier':
        this.deleteSupplier(data);
        break;
    }
  }
}
