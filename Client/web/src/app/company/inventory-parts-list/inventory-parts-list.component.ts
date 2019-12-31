import { Component, OnInit } from '@angular/core';
import { Part, PartsViewModel } from '../../models/part.model';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { PartsService } from '../../admin/parts/parts.service';
import { CompanyService } from '../company.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { FilterOption } from '../../admin/parts/part-list/part-list.component';
import { CustomerService } from '../../admin/customer/customer.service';
import { SupplierService } from '../../admin/supplier/supplier.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../common/constants';

@Component({
  selector: 'app-inventory-parts-list',
  templateUrl: './inventory-parts-list.component.html',
  styleUrls: ['./inventory-parts-list.component.scss']
})
export class InventoryPartsListComponent implements OnInit {

  parts: PartsViewModel[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;
  filter: any;
  filterOption: FilterOption;

  constructor(private service: PartsService, private companyService: CompanyService, private httpLoaderService: httpLoaderService, private customerService: CustomerService,
    private supplierService: SupplierService, private httpLoader: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "Code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "Description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Min Qty", value: "MinQty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Max Qty (Lbs)", value: "MaxQty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Safe Qty", value: "SafeQty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Opening Qty", value: "OpeningQty", isEditable: true, sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "In Hand Qty", value: "QuantityInHand", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "In Transit Qty", value: "IntransitQty", sortable: false, customStyling: 'right' }) );
    //this.columns.push( new DataColumn({ headerText: "Total Qty", value: "Total", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'editOpeningQuantity', icon: 'fa fa-edit' })
    ] }) );
  }

  getAllPartsForCompany() {
    this.httpLoaderService.show();
    this.service.getAllParts(this.currentlyLoggedInCompanyId)
        .subscribe((parts) => {
          var partsToDisplay = [];
          parts.forEach((part) => {
            partsToDisplay.push(new PartsViewModel(part));
          });
          this.parts = partsToDisplay;
        },
        (error) => { console.log(error); },
        () => this.httpLoaderService.hide()
      );
  }

  optionSelected(event) {
    this.parts = [];
    switch (event.target.value) {
      case "1":
        this.filterOption = FilterOption.SelectAll;
        this.getAllPartsForCompany();
        break;
      case "2":
        this.filterOption = FilterOption.Customer;
        this.loadAllCustomers();
        break;
      case "3":
        this.filterOption = FilterOption.Supplier;
        this.loadAllSuppliers();
        break;
    }
  }

  loadAllCustomers() {
    this.httpLoader.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompanyId)
        .subscribe(
          (customers) => this.filter = customers,
          (error) => this.toastr.errorToastr(error),
          () => this.httpLoader.hide()
        );
  }

  loadAllSuppliers() {
    this.httpLoader.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompanyId)
        .subscribe(
          (suppliers) => this.filter = suppliers,
          (error) => this.toastr.errorToastr(error),
          () => this.httpLoader.hide()
        );
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editOpeningQuantity':
        this.service.updateOpeningQuantity(data.part, this.currentlyLoggedInCompanyId, data.part.id, data.OpeningQty)
            .subscribe(() => this.toastr.successToastr('Opening Quantity updated successfully!'),
                      (error) => this.toastr.errorToastr(error.error),
                      () => { this.getAllPartsForCompany() });
        break;
    }
  }

  filterBySelection(event) {
    this.parts = [];
    let selectedValue = parseInt(event.target.value);
    var partsToDisplay = [];
    let observable = this.service.getAllParts(this.currentlyLoggedInCompanyId);

    if (this.filterOption === FilterOption.Customer) {
      observable.subscribe((parts) => {
        var filteredParts = parts.filter(p => p.partCustomerAssignments.find(c => c.customerId === selectedValue));
        filteredParts.forEach((part) => {
          partsToDisplay.push(new PartsViewModel(part));
        });
        this.parts = partsToDisplay;
      });
    } else {
      observable.subscribe((parts) => {
        var filteredParts = parts.filter(p => p.partSupplierAssignments.find(s => s.supplierID === selectedValue));
        filteredParts.forEach((part) => {
          partsToDisplay.push(new PartsViewModel(part));
        });
        this.parts = partsToDisplay;
      });
    }
  }
}