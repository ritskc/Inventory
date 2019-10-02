import { Component, OnInit } from '@angular/core';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { PartsService } from '../../parts/parts.service';
import { SupplierService } from '../../supplier/supplier.service';
import { ActivatedRoute } from '@angular/router';
import { Supplier } from '../../../models/supplier.model';
import { Part } from '../../../models/part.model';
import { CompanyService } from '../../../company/company.service';
import { Customer } from '../../../models/customer.model';
import { CustomerService } from '../../customer/customer.service';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { ClassConstants } from '../../../common/constants';
import { PurchaseOrderDetail, PurchaseOrder, PurchaseOrderTerm } from '../../../models/purchase-order';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Observable } from 'rxjs';
import { httpLoaderService } from '../../../common/services/httpLoader.service';

@Component({
  selector: "app-order-detail",
  templateUrl: "./order-detail.component.html",
  styleUrls: ["./order-detail.component.scss"]
})
export class OrderDetailComponent implements OnInit {
  private orderForm: FormGroup;
  private customers: Customer[] = [];
  private suppliers: Supplier[] = [];
  private parts: Part[] = [];
  private purchaseOrder: PurchaseOrder;
  private gridColumns: DataColumn[] = [];
  private selectedParts: Part[] = [];
  private blankOrders: PurchaseOrder[] = [];

  private reference: string = "";
  private price: number = 0;
  private notes: string = "";
  private quantity: number = 0;
  private dueDate: string = "";
  private partDescription: string = '';
  private currentlyLoaddedInCompanyId: number = 0;
  private blanketPOAdjQty: number = 0;
  private lineNumber: number = 0;
  private blanketPOId: number = 0;
  private isBlanketPO: boolean = false;
  private disableSupplierSelectedPurchaseOrder: boolean = false;
  private disableCustomerSelectedPurchaseOrder: boolean = false;

  constructor(
    private partsService: PartsService, private supplierService: SupplierService, private companyService: CompanyService, private customerService: CustomerService,
    private activatedRoute: ActivatedRoute, private formBuilder: FormBuilder, private toastr: ToastrManager,
    private loaderService: httpLoaderService
  ) {}

  ngOnInit() {
    this.currentlyLoaddedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.purchaseOrder = new PurchaseOrder();
    this.purchaseOrder.companyId = this.currentlyLoaddedInCompanyId;
    
    this.loadSuppliersList();
    this.loadPartsList();
    this.loadCustomersList();

    this.orderForm = this.formBuilder.group({
      poNo: FormControl,
      poDate: FormControl,
      closingDate: FormControl,
      emailIds: FormControl,
      remarks: FormControl,
      partCode: FormControl,
      partDescription: FormControl,
      price: FormControl,
      suppliersList: FormControl,
      customersList: FormControl,
      dueDate: FormControl,
      notes: FormControl,
      reference: FormControl,
      quantity: FormControl,
      paymentTerms: FormControl,
      deliveryTerms: FormControl,
      blanketPOAdjQty: FormControl,
      isBlanketPO: FormControl,
      lineNumber: FormControl,
      blanketPOId: FormControl
    });
  }

  initializeFormForSelection() {
    var userSelection = this.activatedRoute.snapshot.params.from;
    switch (userSelection) {
      case "all":
        this.setFormForAllSelection();
        break;
      case "customer":
        this.setFormForCustomerSelection();
        break;
      case "supplier":
        this.setFormForSupplierSelection();
        break;
    }
  }

  initializePartsGrid() {
    this.gridColumns = [];
    this.gridColumns.push(new DataColumn({headerText: "Part Code", value: "partCode", sortable: true }));
    this.gridColumns.push(new DataColumn({headerText: "Description", value: "description", sortable: true}));
    this.gridColumns.push(new DataColumn({headerText: "Qty", value: "qty", sortable: true}));
    this.gridColumns.push(new DataColumn({ headerText: "Price", value: "unitPrice", sortable: true }));
    this.gridColumns.push(new DataColumn({ headerText: "Total", value: "total", sortable: true }));
    this.gridColumns.push(new DataColumn({headerText: "Due Date", value: "dueDate", sortable: true, isDate: true}));
    this.gridColumns.push(new DataColumn({ headerText: "Notes", value: "note", sortable: false }));
    if (this.SelectedSupplier > -1) {
      this.gridColumns.push(new DataColumn({headerText: "Reference", value: "referenceNo", sortable: false}));
    }
    if (this.SelectedCustomer > -1) {
      this.gridColumns.push(new DataColumn({ headerText: "Blank PO", value: "blanketPOId", sortable: true }));
      this.gridColumns.push(new DataColumn({ headerText: "Open Qty", value: "blanketPOAdjQty", sortable: true }));
      this.gridColumns.push(new DataColumn({ headerText: "Line No", value: "lineNumber", sortable: true }));
    }
    this.gridColumns.push(new DataColumn({headerText: "Actions", isActionColumn: true, actions: [
          new DataColumnAction({actionText: "Remove", actionStyle: ClassConstants.Danger, event: "removeSelectedPart"})
        ]}));
  }

  loadSuppliersList() {
    this.supplierService
      .getAllSuppliers(this.currentlyLoaddedInCompanyId)
      .subscribe((suppliers) => {
        this.suppliers = suppliers;
        this.SelectedSupplier = -1;
      });
  }

  supplierOptionSelected() {
    this.disableCustomerSelectedPurchaseOrder = true;
    this.disableSupplierSelectedPurchaseOrder = false;
  }

  customerOptionSelected() {
    this.disableSupplierSelectedPurchaseOrder = true;
    this.disableCustomerSelectedPurchaseOrder = false;
  }

  loadCustomersList() {
    this.customerService
      .getAllCustomers(this.currentlyLoaddedInCompanyId)
      .subscribe((customers) => {
        this.customers = customers;
        this.SelectedCustomer = -1;
      });
  }

  loadPartsList() {
    this.partsService
      .getAllParts(this.currentlyLoaddedInCompanyId)
      .subscribe(parts => {
        this.parts = parts;
        this.initializeFormForSelection();
      });
  }

  setFormForAllSelection() {
    this.selectedParts = this.parts;
  }

  setFormForCustomerSelection() {
    this.SelectedSupplier = -1;
    if (this.SelectedCustomer > 0) {
      this.purchaseOrder.customerId = +this.SelectedCustomer;
      this.selectedParts = [];
      
      this.parts.forEach(part => {
        if (part.partCustomerAssignments.findIndex(p => p.customerId == this.SelectedCustomer) > -1)
          this.selectedParts.push(part);
      });
      this.initializePartsGrid();
    }
  }

  setFormForSupplierSelection() {
    var selectedSupplier = this.orderForm.get("suppliersList").value;
    this.SelectedCustomer = -1;
    this.purchaseOrder.supplierId = +selectedSupplier;
    if (selectedSupplier > 0) {
      this.selectedParts = [];
      
      this.parts.forEach(part => {
        if (part.partSupplierAssignments.findIndex(p => p.supplierID == selectedSupplier) > -1)
          this.selectedParts.push(part);
      });
      var sequenceNo = 1;
      this.suppliers.find(s => s.id == selectedSupplier).terms.forEach((term) => {
        var poTerm = new PurchaseOrderTerm();
        poTerm.sequenceNo = sequenceNo;
        poTerm.term = term.terms;
        this.purchaseOrder.poTerms.push(poTerm);
        sequenceNo += 1;
      });
      this.initializePartsGrid();
    }
  }

  partSelected(selectedItem: any) {
    this.synchronizeParts(selectedItem);
    var unitPrice: number = 0;

    if(this.SelectedSupplier > -1) {
      var selectedSupplier = this.orderForm.get("suppliersList").value;
      this.selectedParts.forEach(part => {
        unitPrice = part.partSupplierAssignments.find(p => p.supplierID == selectedSupplier).unitPrice;
      });
    }

    if (this.SelectedCustomer > -1) {
      var selectedCustomer = this.orderForm.get("customersList").value;
      this.selectedParts.forEach(part => {
        unitPrice = part.partCustomerAssignments.find(p => p.customerId == selectedCustomer).rate;
      });
    }

    this.orderForm.get("price").setValue(unitPrice);
  }

  synchronizeParts(selectedItem: any) {
    var value = 0;
    switch (selectedItem) {
      case "code":
        value = this.orderForm.get("partCode").value;
        this.orderForm.get("partDescription").setValue(value);
        break;
      case "description":
        value = this.orderForm.get("partDescription").value;
        this.orderForm.get("partCode").setValue(value);
        break;
    }
    this.loadBlankPurchaseOrdersForSelectedCustomer();
  }

  loadBlankPurchaseOrdersForSelectedCustomer() {
    if (this.SelectedCustomer < 1) return;

    this.loaderService.show();
    this.customerService.getAllPurchaseOrders(this.currentlyLoaddedInCompanyId)
      .subscribe((result) => {
        this.blankOrders = [];
        var selectedPartValue = this.orderForm.get("partCode").value;
        result.filter(o => o.customerId == this.SelectedCustomer && o.orderDetails.findIndex(o => o.partId == selectedPartValue) >= 0
          ).map(o => this.blankOrders.push(o));
        this.blanketPOId = -1;
      }, (error) => {
        console.log(error);
      }, () => {
        this.loaderService.hide();
      });
  }

  blankPOSelected() {
    var poId = this.orderForm.get('blanketPOId').value;
    this.loaderService.show();
    this.customerService.getPurchaseOrder(this.currentlyLoaddedInCompanyId, poId)
        .subscribe((result) => {
          var selectedPartValue = this.orderForm.get("partCode").value;
          this.orderForm.get('blanketPOAdjQty').setValue(result.orderDetails.find(p => p.partId == selectedPartValue).part.openingQty);
        }, (error) => { console.log(error) }
        , () => {})
  }

  addPartToOrder() {
    if (this.SelectedSupplier > -1 ) {
      var orderDetail = new PurchaseOrderDetail();
      orderDetail.id = 0;
      orderDetail.poId = 0;
      orderDetail.partId = +this.orderForm.get("partCode").value;
      orderDetail.qty = +this.quantity;
      orderDetail.dueDate = new Date(this.dueDate).toLocaleDateString();
      orderDetail.note = this.notes;
      orderDetail.unitPrice = this.price;
      orderDetail.total = this.quantity * this.price;
      
      var selectedPart = this.selectedParts.find(p => p.id == orderDetail.partId);
      orderDetail.partCode = selectedPart.code;
      orderDetail.description = selectedPart.description;
      orderDetail.referenceNo = this.reference;
      this.purchaseOrder.poDetails.push(orderDetail); 
    }

    if (this.SelectedCustomer > -1) {
      var customerOrderDetail = new PurchaseOrderDetail();
      customerOrderDetail.id = 0;
      customerOrderDetail.orderId = 0;
      customerOrderDetail.partId = +this.orderForm.get("partCode").value;
      customerOrderDetail.qty = +this.quantity;
      customerOrderDetail.dueDate = new Date(this.dueDate).toLocaleDateString();
      customerOrderDetail.note = this.notes;
      customerOrderDetail.unitPrice = this.price;
      customerOrderDetail.total = this.quantity * this.price;
      
      var selectedPart = this.selectedParts.find(p => p.id == customerOrderDetail.partId);
      customerOrderDetail.partCode = selectedPart.code;
      customerOrderDetail.description = selectedPart.description;
      customerOrderDetail.blanketPOId = this.blanketPOId;
      customerOrderDetail.lineNumber = this.lineNumber;
      customerOrderDetail.blanketPOAdjQty = this.blanketPOAdjQty;

      this.purchaseOrder.orderDetails.push(customerOrderDetail); 
      this.purchaseOrder.isBlanketPO = this.isBlanketPO;
    }
  }

  addMoreTermAndCondition() {
    var purchaseOrder = new PurchaseOrderTerm();
    purchaseOrder.sequenceNo = this.purchaseOrder.poTerms.length + 1;
    this.purchaseOrder.poTerms.push(purchaseOrder);
  }

  removePart(data) {
    var index = this.purchaseOrder.poDetails.indexOf(data);
    this.purchaseOrder.poDetails.splice(index, 1);
  }

  removeTermAndCondition(index) {
    this.purchaseOrder.poTerms.splice(index, 1);
  }

  get SelectedSupplier() {
    return this.orderForm ? this.orderForm.get("suppliersList").value : null;
  }

  set SelectedSupplier(value: any) {
    this.orderForm.get('suppliersList').setValue(value);
  }

  get SelectedCustomer() {
    return this.orderForm ? this.orderForm.get('customersList').value : null;
  }

  set SelectedCustomer(value: any) {
    this.orderForm.get('customersList').setValue(value);
  }

  get disabled() {
    return this.SelectedCustomer == -1 && this.SelectedSupplier == -1;
  }

  save() {
    var observableResult: any;
    if (this.SelectedCustomer > -1) {
      observableResult = this.customerService.savePurchaseOrder(this.purchaseOrder);
    } else {
      observableResult = this.supplierService.savePurchaseOrder(this.purchaseOrder);
    }
    observableResult.subscribe((result) => {
      this.toastr.successToastr('Details saved successfully.');
    }, (error) => {
      this.toastr.errorToastr('Could not save details. Please try again & contact administrator if the problem persists!!')
      console.log(error);
    });
  }
}