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
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import * as DateHelper from '../../common/helpers/dateHelper';

@Component({
  selector: 'app-inventory-parts-list',
  templateUrl: './inventory-parts-list.component.html',
  styleUrls: ['./inventory-parts-list.component.scss']
})
export class InventoryPartsListComponent implements OnInit {

  parts: PartsViewModel[] = [];
  columns: DataColumn[] = [];
  columnsForModal: DataColumn[] = [];
  columnsForSecondaryGridInModal: DataColumn[] = [];
  currentlyLoggedInCompanyId: number = 0;
  filter: any;
  filterOption: FilterOption;
  showModal: boolean = false;
  showOpenOrdersModal: boolean = false;
  showInTransitModal: boolean = false;
  showLatestShipmentsModal: boolean = false;
  showSupplierOpnePoModal: boolean = false;
  selectedPartIdForAdjustment: number = 0;
  direction: string = 'in';
  notes: string = '';
  adjustedQty: number = 0;
  dataForModal: any;
  dataForSecondaryGridInModal: any;
  monthlyCustomer: boolean = false;
  showOtherColumns: boolean = false;
  from: Date;

  constructor(private service: PartsService, private companyService: CompanyService, private httpLoaderService: httpLoaderService, private customerService: CustomerService,
    private supplierService: SupplierService, private httpLoader: httpLoaderService, private toastr: ToastrManager, private route: Router) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.getAllPartsForCompany();
  }

  initializeGridColumns() {
    this.columns = [];
    if (this.showOtherColumns || !this.monthlyCustomer) {
      this.columns.push( new DataColumn({ headerText: "Code", value: "Code", columnName: 'PartCode', sortable: true }) );
      this.columns.push( new DataColumn({ headerText: "Description", value: "Description", columnName: 'PartDescription', sortable: true, customStyling: 'column-width-150' }) );
      this.columns.push( new DataColumn({ headerText: "Opening Qty", value: "OpeningQty", columnName: 'OpeningQty', customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Open + In Hand", value: "QuantityInHand", columnName: 'OpenInHandQty', customStyling: 'right', sortable: false, hasAdditionalAction: true, additionalActionName: 'showLatestShipments' }) );
      this.columns.push( new DataColumn({ headerText: "In Transit", value: "IntransitQty", columnName: 'InTransitQty', customStyling: 'right', sortable: false, hasAdditionalAction: true, additionalActionName: 'showInTransitQty' }) );
      this.columns.push( new DataColumn({ headerText: "Open Order", value: "OpenOrderQty", columnName: 'OpenOrder', isEditable: false, customStyling: 'right', sortable: false, hasAdditionalAction: true, additionalActionName: 'showOpenOrders' }) );
      this.columns.push( new DataColumn({ headerText: "Safe Qty", value: "SafeQty", columnName: 'SafeQty', sortable: false, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Supp Open PO", value: "SupplierOpenPoQty", columnName: 'SupplierOpenPO', customStyling: 'right', sortable: false, hasAdditionalAction: true, additionalActionName: 'showSupplierOpenPO' }) );
      this.columns.push( new DataColumn({ headerText: "Min Qty", value: "MinQty", columnName: 'MinQty', sortable: false, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Max Qty", value: "MaxQty", columnName: 'MaxQty', sortable: false, customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Cust Price", value: "customerPrice", customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Supp Price", value: "supplierPrice", customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Supp Code", value: "supplierCode" }) );
      this.columns.push( new DataColumn({ headerText: "Double Price", value: "isDoublePricingAllowed", isBoolean: true, customStyling: 'center', isDisabled: true }) );
      this.columns.push( new DataColumn({ headerText: "Future Price", value: "futurePrice", customStyling: 'right' }) );
      this.columns.push( new DataColumn({ headerText: "Efctv Price", value: "currentPricingInEffectQty", customStyling: 'right' }) );
    } else {
      this.columns.push( new DataColumn({ headerText: "Code", value: "Code", columnName: 'PartCode', sortable: true }) );
      this.columns.push( new DataColumn({ headerText: "Description", value: "Description", columnName: 'PartDescription', sortable: true, customStyling: 'column-width-150' }) );
      this.columns.push( new DataColumn({ headerText: "Opening Qty", value: "monthlyOpeningQty", columnName: 'OpeningQty', sortable: false, hasAdditionalAction: true, additionalActionName: 'showLatestShipments' }) );
      this.columns.push( new DataColumn({ headerText: "Shipped", value: "shippedQty", columnName: 'Shipped' }) );
      this.columns.push( new DataColumn({ headerText: "Invoiced", value: "invoiceQty", columnName: 'Invoiced' }) );
      this.columns.push( new DataColumn({ headerText: "Return Qty", value: "monthlyReturnQty", columnName: 'ReturnQty' }) );
      this.columns.push( new DataColumn({ headerText: "Excess Qty", value: "monthlyExcessQty", columnName: 'ExcessQty' }) );
      this.columns.push( new DataColumn({ headerText: "Reject Qty", value: "monthlyRejectQty", columnName: 'RejectQty' }) );
      this.columns.push( new DataColumn({ headerText: "In Transit", value: "IntransitQty", columnName: 'InTransitQty', sortable: false, hasAdditionalAction: true, additionalActionName: 'showInTransitQty' }) );
      this.columns.push( new DataColumn({ headerText: "Closing Qty", value: "monthlyClosingQty", columnName: 'ClosingQty' }) );
    }
    this.columns.push( new DataColumn({ headerText: "Action", columnName: 'Action', value: "Action", isActionColumn: true, customStyling: 'center column-width-100', actions: [
      new DataColumnAction({ actionText: 'Adjust', actionStyle: ClassConstants.Primary, event: 'adjustOpeningQuantity' })
    ] }) );
}

  getAllPartsForCompany(dateRangeSelected: boolean = false) {
    this.httpLoaderService.show();
    var partObservable: Observable<Part[]> = dateRangeSelected ? this.service.getInventoryForDateRange(this.currentlyLoggedInCompanyId, this.from)
                        : this.service.getAllParts(this.currentlyLoggedInCompanyId)

        partObservable
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
    this.httpLoader.show();
    this.service.adjustPart(this.selectedPartIdForAdjustment, this.direction, this.notes, this.currentlyLoggedInCompanyId, this.adjustedQty, this.monthlyCustomer)
        .subscribe(() => {
          this.showModal = false;
          this.toastr.successToastr('Part inventory quantity adjusted successfully!!');
        }, 
        (error) => this.toastr.errorToastr(error.error),
        () => { 
          this.httpLoader.hide();
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
        this.monthlyCustomer = this.filter.find(c => c.id == selectedValue).invoicingtypeid == 3;
        this.initializeGridColumns();
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
      case 'showSupplierOpenPO':
        this.showSupplierOpenPo(event.data);
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
    this.columnsForModal.push( new DataColumn({ headerText: "Due Date", value: "dueDate", isDate: true }) );
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

  showSupplierOpenPo(data: any) {
    this.showSupplierOpnePoModal = true;

    this.columnsForModal = [];
    this.columnsForModal.push( new DataColumn({ headerText: "Sr No", value: "srNo", customStyling: 'right' }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Supplier", value: "supplierName" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Code", value: "code" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Part Description", value: "description" }) );
    this.columnsForModal.push( new DataColumn({ headerText: 'PO No', value: 'poNo' }));
    this.columnsForModal.push( new DataColumn({ headerText: "Reference", value: "referenceNo" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", customStyling: 'right' }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Due", value: "dueDate", isDate: true }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Ack Date", value: "acknowledgeDate", isDate: true }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Open Qty", value: "openQty" }) );
    this.columnsForModal.push( new DataColumn({ headerText: "Note", value: "note" }) );

    this.service.getPartsStatus(this.currentlyLoggedInCompanyId, data.part.id, 'SupplierOpenPO')
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

    this.columnsForSecondaryGridInModal = [];
    this.columnsForSecondaryGridInModal.push( new DataColumn({ headerText: "Supplier", value: "supplierName" }) );
    this.columnsForSecondaryGridInModal.push( new DataColumn({ headerText: "Part Code", value: "code" }) );
    this.columnsForSecondaryGridInModal.push( new DataColumn({ headerText: "Part Description", value: "description" }) );
    this.columnsForSecondaryGridInModal.push( new DataColumn({ headerText: "Invoice No", value: "invoiceNo" }) );
    this.columnsForSecondaryGridInModal.push( new DataColumn({ headerText: "Received Date", value: "receivedDate", isDate: true }) );
    this.columnsForSecondaryGridInModal.push( new DataColumn({ headerText: "Qty", value: "qty" }) );

    this.service.getPartsStatus(this.currentlyLoggedInCompanyId, data.part.id, 'LatestShipment')
        .subscribe(data => {
          this.dataForModal = data;
        });

    this.service.getPartsStatus(this.currentlyLoggedInCompanyId, data.part.id, 'LatestReceived')
    .subscribe(data => {
      this.dataForSecondaryGridInModal = data;
    });
  }

  showOtherColumnsForInventory() {
    this.initializeGridColumns();
  }
}