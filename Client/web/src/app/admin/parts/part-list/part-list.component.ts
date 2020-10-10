import { Component, OnInit } from '@angular/core';
import { Part, PartCosting } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { UserAction } from '../../../models/enum/userAction';
import { CustomerService } from '../../customer/customer.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { SupplierService } from '../../supplier/supplier.service';
import { ClassConstants } from '../../../common/constants';
import readXlsxFile from 'read-excel-file';

@Component({
  selector: 'app-part-list',
  templateUrl: './part-list.component.html',
  styleUrls: ['./part-list.component.scss']
})
export class PartListComponent implements OnInit {

  parts: Part[] = [];
  columns: DataColumn[] = [];
  columnsForCosting: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;
  filter: any;
  filterOption: FilterOption;
  showModal: boolean = false;
  showModalForImportStockPrices: boolean = false;
  selectedPartForCostingUpdate: Part;
  stockPrices: PartCosting[] = [];

  constructor(private service: PartsService, private activatedRoute: ActivatedRoute, private router: Router, private customerService: CustomerService,
              private httpLoader: httpLoaderService, private companyService: CompanyService, private toastr: ToastrManager, private supplierService: SupplierService) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
    this.filterOption = FilterOption.SelectAll;
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "description", sortable: true, customStyling: 'column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Weight (Kgs)", value: "weightInKg", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Weight (Lbs)", value: "weightInLb", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Min Qty", value: "minQty", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Max Qty", value: "maxQty", sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "safeQty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Supplier Code", value: "supplierCode", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Supp Price", value: "supplierPrice", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Cust Price", value: "customerPrice", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Double Price", value: "isDoublePricingAllowed", sortable: true, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Active", value: "isActive", sortable: true, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Sample", value: "isSample", sortable: true, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editPart', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: 'Stock', actionStyle: ClassConstants.Primary, event: 'showUpdateCostModal' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deletePart', icon: 'fa fa-trash' })
    ] }) );    
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

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'editPart':
        this.rowSelected(data);
        break;
      case 'deletePart':
        this.deletePart(data);
        break;
      case 'showUpdateCostModal':
        this.showUpdateCostModal(data);
        break;
    }
  }

  actionButtonClickedInCosting(data) {
    switch(data.eventName) {
      case 'deletePartCosting':
        this.deletePartCosting(data);
        break;
    }
  }

  showUpdateCostModal(data) {
    this.showModal = true;
    this.columnsForCosting = [];
    this.columnsForCosting.push( new DataColumn({ headerText: "Supplier Price", value: "supplierPrice", isEditable: true }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Customer Price", value: "customerPrice", isEditable: true }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Qty", value: "qty", isEditable: true }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deletePartCosting', icon: 'fa fa-trash' })
    ] }) );
    this.selectedPartForCostingUpdate = data;
    this.stockPrices = this.selectedPartForCostingUpdate.stockPrices ? this.selectedPartForCostingUpdate.stockPrices: [];
  }

  updateCosting() {
    this.selectedPartForCostingUpdate.stockPrices = this.stockPrices;
    this.httpLoader.show();
    this.service.updatePartCosting(this.currentlyLoggedInCompanyId, this.selectedPartForCostingUpdate)
        .subscribe(
          () => {
            this.toastr.successToastr('Part cost details updated successfully');
            this.getAllPartsForCompany();
            this.showModal = false;
            this.selectedPartForCostingUpdate = new Part;
          }, 
          (error) => this.toastr.errorToastr(error.error),
          () => this.httpLoader.hide()
        );
  }

  uploadFile(files: FileList) {
    this.columnsForCosting = [];
    this.columnsForCosting.push( new DataColumn({ headerText: "Part Code", value: "PartCode" }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Supplier Price", value: "supplierPrice", isEditable: true }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Customer Price", value: "customerPrice", isEditable: true }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Qty", value: "qty", isEditable: true }) );
    this.columnsForCosting.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deletePartCosting', icon: 'fa fa-trash' })
    ] }) );
    
    readXlsxFile(files[0]).then((rows) => {
      this.extractDataFromFile(rows);
    });
  }

  extractDataFromFile(rows: any) {    
    this.stockPrices = [];
    for (let index = 1; index < rows.length; index++) {
      var stockPrice = new PartCosting();
      stockPrice.PartCode = rows[index][0];
      stockPrice.customerPrice = rows[index][1];
      stockPrice.supplierPrice = rows[index][2];
      stockPrice.qty = rows[index][3];
      this.stockPrices.push(stockPrice);
    }
  }

  updatePartsStockPricing() {
    if (this.stockPrices.length == 0) {
      this.toastr.errorToastr('There is no data to upload stock price');
      return;
    }

    var partsUpdated = 0;
    this.httpLoader.show();
    this.service.updatePartCostingByPart(this.currentlyLoggedInCompanyId,  this.stockPrices)
          .subscribe(() => {
            this.showModalForImportStockPrices = false;
            this.httpLoader.hide();
            this.toastr.successToastr('Stock pricing uploaded successfully');
          },
          (error) => this.toastr.errorToastr(error.error),
          () => this.httpLoader.hide());
  }

  deletePartCosting(data) {
    this.stockPrices.splice(this.stockPrices.findIndex(s => s == data), 1);
  }

  addPartCosting(data) {
    var newPartCosting = new PartCosting();
    newPartCosting.PartId = this.selectedPartForCostingUpdate.id;
    this.stockPrices.push(newPartCosting);
  }

  deletePart(data) {
    if (confirm('Are you sure you want to remove this part?')) {
      this.service.delete(data.id)
          .subscribe(() => this.toastr.successToastr('Removed the part successfully'),
            (error) => this.toastr.errorToastr(error.error)
          );
    }
  }
}

export enum FilterOption {
  SelectAll = 1,
  Customer,
  Supplier,
  Warehouse
}