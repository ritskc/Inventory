import { Component, OnInit } from '@angular/core';
import { Part } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';
import { CustomerService } from '../../customer/customer.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { SupplierService } from '../../supplier/supplier.service';

@Component({
  selector: 'app-part-list',
  templateUrl: './part-list.component.html',
  styleUrls: ['./part-list.component.scss']
})
export class PartListComponent implements OnInit {

  parts: Part[] = [];
  columns: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;
  filter: any;
  filterOption: FilterOption;

  constructor(private service: PartsService, private activatedRoute: ActivatedRoute, private router: Router, private customerService: CustomerService,
              private httpLoader: httpLoaderService, private companyService: CompanyService, private toastr: ToastrManager, private supplierService: SupplierService) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
    this.filterOption = FilterOption.SelectAll;
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "code", isLink: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Weight (Kgs)", value: "weightInKg", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Weight (Lbs)", value: "weightInLb", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Min Qty", value: "minQty", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Max Qty", value: "maxQty", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "safeQty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Supplier Code", value: "supplierCode", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Supp Price", value: "supplierPrice", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Cust Price", value: "customerPrice", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Active", value: "isActive", sortable: true, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Sample", value: "isSample", sortable: true, isBoolean: true, customStyling: 'center', isDisabled: true }) );
  }

  getAllPartsForCompany() {
    this.httpLoader.show();
    this.service.getAllParts(this.currentlyLoggedInCompanyId)
        .subscribe((parts) => {
          this.parts = parts;
          this.httpLoader.hide();
        },
        (error) => {
          console.log(error);
          this.httpLoader.hide();
        }
      );
  }

  rowSelected(row) {
    this.router.navigateByUrl(`/parts/detail/${ UserAction.Edit }/${row.id}`);
  }

  addPart() {
    this.router.navigateByUrl(`/parts/detail/${ UserAction.Add }/#`);
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
        this.getAllPartsForSelectedCustomer();
        break;
      case "3":
        this.filterOption = FilterOption.Supplier;
        this.getAllPartsForSelectedSupplier();
        break;
    }
  }

  getAllPartsForSelectedCustomer() {
    this.httpLoader.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompanyId)
        .subscribe(
          (customers) => this.filter = customers,
          (error) => this.toastr.errorToastr(error),
          () => this.httpLoader.hide()
        );
  }

  getAllPartsForSelectedSupplier() {
    this.httpLoader.show();
    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompanyId)
        .subscribe(
          (suppliers) => this.filter = suppliers,
          (error) => this.toastr.errorToastr(error),
          () => this.httpLoader.hide()
        );
  }

  filterBySelection(event) {
    let selectedValue = parseInt(event.target.value);
    let observable = this.service.getAllParts(this.currentlyLoggedInCompanyId);

    if (this.filterOption === FilterOption.Customer) {
      observable.subscribe((parts) => {
        this.parts = parts.filter(p => p.partCustomerAssignments.find(c => c.customerId === selectedValue));
      });
    } else {
      observable.subscribe((parts) => {
        this.parts = parts.filter(p => p.partSupplierAssignments.find(s => s.supplierID === selectedValue));
      });
    }
  }
}

export enum FilterOption {
  SelectAll = 1,
  Customer,
  Supplier
}