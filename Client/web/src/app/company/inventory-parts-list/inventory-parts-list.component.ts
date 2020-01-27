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
  columnsForModal: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;
  filter: any;
  filterOption: FilterOption;
  showModal: boolean = false;
  showOpenOrdersModal: boolean = false;
  showInTransitModal: boolean = false;
  showLatestShipmentsModal: boolean = false;
  selectedPartIdForAdjustment: number = 0;
  direction: string = 'in';
  notes: string = '';
  adjustedQty: number = 0;
  dataForModal: any;

  constructor(private service: PartsService, private companyService: CompanyService, private httpLoaderService: httpLoaderService, private customerService: CustomerService,
    private supplierService: SupplierService, private httpLoader: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Code", value: "Code", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "Description", sortable: true, customStyling: 'column-width-150' }) );
    this.columns.push( new DataColumn({ headerText: "Min Qty", value: "MinQty", sortable: false, customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Max Qty (Lbs)", value: "MaxQty", sortable: false, customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Safe Qty", value: "SafeQty", sortable: false, customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Opening Qty", value: "OpeningQty", isEditable: true, sortable: false, customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Open Order", value: "OpenOrderQty", isEditable: false, sortable: false, hasAdditionalAction: true, additionalActionName: 'showOpenOrders', customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Open + In Hand", value: "QuantityInHand", sortable: false, hasAdditionalAction: true, additionalActionName: 'showLatestShipments', customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "In Transit", value: "IntransitQty", sortable: false, hasAdditionalAction: true, additionalActionName: 'showInTransitQty', customStyling: 'right column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Action", isActionColumn: true, customStyling: 'center column-width-100', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'adjustOpeningQuantity', icon: 'fa fa-adjust' }),
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
      case 'adjustOpeningQuantity':
        this.selectedPartIdForAdjustment = data.part.id;
        this.showModal = true;
        break;
    }
  }

  savePartAdjustment() {
    this.service.adjustPart(this.selectedPartIdForAdjustment, this.direction, this.notes, this.currentlyLoggedInCompanyId, this.adjustedQty)
        .subscribe(() => {
          this.showModal = false;
          this.toastr.successToastr('Part inventory quantity adjusted successfully!!');
        }, 
        (error) => this.toastr.errorToastr(error.error),
        () => { 
          this.getAllPartsForCompany(); 
          this.direction = 'in';
          this.adjustedQty = 0;
          this.notes = '';
        });
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

  additionalEventEmitted(event: any) {
    switch (event.eventName) {
      case 'showOpenOrders':
        this.showOpenOrders(event.data);
        break;
      case 'showInTransitQty':
        this.showInTransitQty(event.data);
        break;
      case 'showLatestShipments':
        this.showLatestShipments(event.data);
        break;
    }
  }

  showOpenOrders(data: any) {
    this.showOpenOrdersModal = true;
    
    this.columnsForModal = [];
    this.columnsForModal.push( new DataColumn({ headerText: "Customer", value: "customerName" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Code", value: "code" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Description", value: "description" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "PO No", value: "poNo" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "PO Date", value: "poDate", isDate: true }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Open Qty", value: "openQty" }) );

    this.service.getPartsStatus(this.currentlyLoggedInCompanyId, data.part.id, 'OpenOrder')
        .subscribe(data => {
          this.dataForModal = data;
        });
  }

  showInTransitQty(data: any) {
    this.showOpenOrdersModal = true;

    this.columnsForModal = [];
    this.columnsForModal.push( new DataColumn({ headerText: "Part Code", value: "code" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Description", value: "description" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Supplier", value: "supplierName" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Invoice", value: "invoiceNo" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", isDate: true }) );
    this.columnsForModal.push( new DataColumn({ headerText: "ETA", value: "eta", isDate: true }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Po No", value: "poNo" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Qty", value: "qty" }) );

    this.service.getPartsStatus(this.currentlyLoggedInCompanyId, data.part.id, 'InTransit')
        .subscribe(data => {
          this.dataForModal = data;
        });
  }

  showLatestShipments(data: any) {
    this.showLatestShipmentsModal = true;

    this.columnsForModal = [];
    this.columnsForModal.push( new DataColumn({ headerText: "Customer", value: "customerName" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Code", value: "code" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Description", value: "description" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Packing Slip", value: "packingSlipNo" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Shipping Date", value: "shippingDate", isDate: true }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Qty", value: "qty" }) );

    this.service.getPartsStatus(this.currentlyLoggedInCompanyId, data.part.id, 'LatestShipment')
        .subscribe(data => {
          this.dataForModal = data;
        });
  }
}