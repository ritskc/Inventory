import { Component, OnInit } from '@angular/core';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { PartsService } from '../../parts/parts.service';
import { SupplierService } from '../../supplier/supplier.service';
import { ActivatedRoute, Router } from '@angular/router';
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
import * as DateHelper from '../../../common/helpers/dateHelper';
import { Company } from '../../../models/company.model';
import { UserAction } from '../../../models/enum/userAction';
import { FileUploadService } from '../../../common/services/file-upload.service';

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
  private currentlyLoggedInCompany: Company;
  private reference: string = "";
  private price: number = 0;
  private notes: string = "";
  private quantity: number = 0;
  private dueDate: string = "";
  private partDescription: string = '';
  private currentlyLoaddedInCompanyId: number = 0;
  private blanketPOAdjQty: number = 0;
  private qtyInHand: number = 0;
  private lineNumber: number = 0;
  private blanketPOId: number = 0;
  private isBlanketPO: boolean = false;
  private idFromUrl: number = 0;
  private formMode: UserAction = UserAction.Add;
  private orderFormMode: OrderFormMode;
  private documents = [];

  constructor(
    private partsService: PartsService, private supplierService: SupplierService, private companyService: CompanyService, private customerService: CustomerService,
    private activatedRoute: ActivatedRoute, private formBuilder: FormBuilder, private toastr: ToastrManager,
    private loaderService: httpLoaderService, private router: Router, private fileService: FileUploadService
  ) {}

  ngOnInit() {
    this.currentlyLoaddedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.purchaseOrder = new PurchaseOrder();
    this.purchaseOrder.companyId = this.currentlyLoaddedInCompanyId;
    
    this.loadPartsList();

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
      qtyInHand: FormControl,
      isBlanketPO: FormControl,
      lineNumber: FormControl,
      blanketPOId: FormControl
    });

    this.initializeFormForSelection();
  }

  initializeFormForSelection() {
    var userSelection = this.activatedRoute.snapshot.params.from;
    this.idFromUrl = parseInt(this.activatedRoute.snapshot.params.id);
    var orderId = this.activatedRoute.snapshot.params.orderId;
    if (orderId > 0) {
      this.formMode = UserAction.Edit;
    }

    switch (userSelection) {
      case "all":
        this.setFormForAllSelection();
        break;
      case "customer":
        this.orderFormMode = OrderFormMode.Customer;
        this.loadCustomersList();
        this.setFormForCustomerSelection();
        this.setFormForCustomerOrderEdit();
        break;
      case "supplier":
        this.orderFormMode = OrderFormMode.Supplier;
        this.orderForm.get('poNo').disable();
        this.loadSuppliersList();
        this.setFormForSupplierSelection();
        this.setFormForSupplierOrderEdit();
        break;
    }
    this.orderForm.get("partCode").setValue(-1);
    this.orderForm.get("partDescription").setValue(-1);
    this.loadDefaultValues();
  }

  initializePartsGrid() {
    this.gridColumns = [];
    this.gridColumns.push(new DataColumn({headerText: "Sr No", value: "srNo", sortable: false, customStyling: 'right' }));
    this.gridColumns.push(new DataColumn({headerText: "Part Code", value: "partCode", sortable: true }));
    this.gridColumns.push(new DataColumn({headerText: "Description", value: "description", sortable: true, customStyling: 'column-width-150' }));
    this.gridColumns.push(new DataColumn({headerText: "Qty", value: "qty",  isEditable: true, customStyling: 'right column-width-50'}));
    this.gridColumns.push(new DataColumn({ headerText: "Price", value: "unitPrice", customStyling: 'right column-width-50', isEditable: true }));
    this.gridColumns.push(new DataColumn({ headerText: "Total", value: "total", customStyling: 'right' }));
    this.gridColumns.push(new DataColumn({headerText: "Due Date", value: "dueDate", sortable: true, isEditableDate: true}));
    this.gridColumns.push(new DataColumn({ headerText: "Notes", value: "note", isEditable: true }));
    this.gridColumns.push(new DataColumn({ headerText: "Pack Slip", value: "packingSlipNo", sortable: false }));
    this.gridColumns.push(new DataColumn({ headerText: "Force Close", value: "isForceClosed", isBoolean: true, customStyling: 'center' }));
    if (this.SelectedSupplier > -1) {
      this.gridColumns.push(new DataColumn({headerText: "Reference", value: "referenceNo", isEditable: true }));
    }
    if (this.SelectedCustomer > -1) {
      this.gridColumns.push(new DataColumn({ headerText: "Open Qty", value: "openQty", customStyling: 'right' }));
      this.gridColumns.push(new DataColumn({ headerText: "Line", value: "lineNumber", isEditable: true, customStyling: 'right column-width-50' }));
    }
    this.gridColumns.push(new DataColumn({headerText: "Actions", isActionColumn: true, customStyling: 'center', actions: [
          new DataColumnAction({actionText: "", actionStyle: ClassConstants.Danger, icon: 'fa fa-trash', event: "removeSelectedPart"})
        ]}));
  }

  loadSuppliersList() {
    this.supplierService
      .getAllSuppliers(this.currentlyLoaddedInCompanyId)
      .subscribe((suppliers) => {
        this.suppliers = suppliers;
        this.SelectedSupplier = -1;

        var suppliedSupplierId = parseInt(this.activatedRoute.snapshot.params.id);
        if (suppliedSupplierId > -1) {
          this.SelectedSupplier = suppliedSupplierId;
          this.orderForm.get("suppliersList").setValue(suppliedSupplierId);
        }
      });
  }

  loadCustomersList() {
    this.customerService
      .getAllCustomers(this.currentlyLoaddedInCompanyId)
      .subscribe((customers) => {
        this.customers = customers;
        this.SelectedCustomer = -1;

        var suppliedCustomerId = parseInt(this.activatedRoute.snapshot.params.id);
        if (suppliedCustomerId > -1) {
          this.orderForm.get("customersList").setValue(this.SelectedCustomer);
          this.SelectedCustomer = suppliedCustomerId;
        }
      });
  }

  loadPartsList() {
    this.loaderService.show();
    this.partsService
      .getAllParts(this.currentlyLoaddedInCompanyId)
      .subscribe(parts => {
        this.parts = parts;
        this.initializeFormForSelection();
        this.loaderService.hide();
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
    this.SelectedCustomer = -1;
    this.purchaseOrder.supplierId = this.idFromUrl;
    if (this.idFromUrl > 0) {
      this.selectedParts = [];
      
      this.parts.forEach(part => {
        if (part.partSupplierAssignments.findIndex(p => p.supplierID == this.idFromUrl) > -1)
          this.selectedParts.push(part);
      });
      var sequenceNo = 1;
      var supplier = this.suppliers.find(s => s.id == this.idFromUrl);
      if (supplier) {
        this.companyService.getCompany(this.currentlyLoaddedInCompanyId).subscribe(company => {
          this.currentlyLoggedInCompany = company
          setTimeout(() => {
            this.purchaseOrder.emailIds = `${ supplier.emailID }, ${ this.currentlyLoggedInCompany.eMail }`;
          }, 100);
        });
        supplier.terms.forEach((term) => {
          var poTerm = new PurchaseOrderTerm();
          poTerm.sequenceNo = sequenceNo;
          poTerm.term = term.terms;
          this.purchaseOrder.poTerms.push(poTerm);
          sequenceNo += 1;
        });
      }
      if (this.formMode === UserAction.Add) {
        this.supplierService.getNewPurchaseOrderNumber(this.currentlyLoaddedInCompanyId, DateHelper.getToday()).subscribe(po => 
          this.purchaseOrder.poNo = po.entityNo
        );
      }
      this.initializePartsGrid();
    }
  }

  setFormForSupplierOrderEdit() {
    var orderId = this.activatedRoute.snapshot.params.orderId;
    if (this.formMode === UserAction.Edit ) {
      this.supplierService.getPurchaseOrder(this.currentlyLoaddedInCompanyId, orderId)
        .subscribe(order => {
          this.purchaseOrder = order;
          this.purchaseOrder.poDate = DateHelper.formatDate(new Date(this.purchaseOrder.poDate));
          this.purchaseOrder.dueDate = DateHelper.formatDate(new Date(this.purchaseOrder.dueDate));
          this.purchaseOrder.poDetails.forEach(poDetail => {
            poDetail.partCode = poDetail.part.code;
            poDetail.description = poDetail.part.description;
            poDetail.total = (poDetail.qty * poDetail.unitPrice).toFixed(2);
            poDetail.note = poDetail.note;
            poDetail.referenceNo = poDetail.referenceNo;
          });
        });
    }
  }

  setFormForCustomerOrderEdit() {
    var orderId = this.activatedRoute.snapshot.params.orderId;
    if (this.formMode === UserAction.Edit ) {
      this.customerService.getPurchaseOrder(this.currentlyLoaddedInCompanyId, orderId)
        .subscribe(order => {
          this.purchaseOrder = order;
          this.purchaseOrder.poDate = DateHelper.formatDate(new Date(this.purchaseOrder.poDate));
          this.purchaseOrder.dueDate = DateHelper.formatDate(new Date(this.purchaseOrder.dueDate));
          this.purchaseOrder.orderDetails.forEach(od => {
            od.partCode = od.part.code;
            od.description = od.part.description;
            od.openQty = od.qty - od.shippedQty;
            od.total = (od.qty * od.unitPrice).toFixed(2);
          });
        });
    }
  }

  partSelected(selectedItem: any) {
    this.synchronizeParts(selectedItem);
    var unitPrice: number = 0;

    var partSelected = this.orderForm.get("partDescription").value;

    if(this.SelectedSupplier > -1) {
      var selectedSupplier = this.orderForm.get("suppliersList").value;
      unitPrice = this.parts.find(p => p.id == partSelected).partSupplierAssignments.find(s => s.supplierID == selectedSupplier).unitPrice;
    }

    if (this.SelectedCustomer > -1) {
      var selectedCustomer = this.orderForm.get("customersList").value;
      unitPrice = this.parts.find(p => p.id == partSelected).partCustomerAssignments.find(c => c.customerId == selectedCustomer).rate;
      //var qtyInHand = this.parts.find(p => p.id == partSelected).qtyInHand;
      var p = this.parts.find(p => p.id == partSelected)
      var qtyInHand = p.qtyInHand + p.openingQty;
      this.orderForm.get("qtyInHand").setValue(qtyInHand);
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
    if (this.quantity < 1) {
      this.toastr.warningToastr('Part quantity should be more than 0 (zero)');
      return;
    }

    if (this.SelectedSupplier > -1 ) {
      var orderDetail = new PurchaseOrderDetail();
      orderDetail.id = 0;
      orderDetail.poId = 0;
      orderDetail.partId = +this.orderForm.get("partCode").value;
      orderDetail.qty = +this.quantity;
      orderDetail.ackQty = orderDetail.qty;
      orderDetail.dueDate = this.dueDate;
      orderDetail.note = this.notes;
      orderDetail.unitPrice = this.price;
      orderDetail.total = (this.quantity * this.price).toFixed(2);
      
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
      customerOrderDetail.dueDate = this.dueDate;
      customerOrderDetail.note = this.notes;
      customerOrderDetail.unitPrice = this.price;
      customerOrderDetail.total = (this.quantity * this.price).toFixed(2);
      
      var selectedPart = this.selectedParts.find(p => p.id == customerOrderDetail.partId);
      customerOrderDetail.partCode = selectedPart.code;
      customerOrderDetail.description = selectedPart.description;
      customerOrderDetail.blanketPOId = this.blanketPOId;
      customerOrderDetail.lineNumber = this.lineNumber;
      customerOrderDetail.blanketPOAdjQty = this.blanketPOAdjQty;
      customerOrderDetail.openQty = customerOrderDetail.qty - customerOrderDetail.shippedQty;

      this.purchaseOrder.orderDetails.push(customerOrderDetail); 
      this.purchaseOrder.isBlanketPO = this.isBlanketPO;
    }

    this.resetSerialNumber();
    this.clearPartDetailSection();
  }

  resetSerialNumber() {
    var srNo: number = 0;
    if (this.purchaseOrder.poDetails)
      this.purchaseOrder.poDetails.forEach(item => item.srNo = ++srNo);
    if (this.purchaseOrder.orderDetails)
      this.purchaseOrder.orderDetails.forEach(item => item.srNo = ++srNo);
  }

  clearPartDetailSection() {
    this.orderForm.get('partCode').setValue(-1);
    this.orderForm.get("partDescription").setValue(-1);
    this.quantity = 0;
    this.notes = '';
    this.price = 0;
    this.reference = '';
    this.blanketPOId = -1;
    this.lineNumber = 0;
    this.blanketPOAdjQty = 0;
  }

  addMoreTermAndCondition() {
    var purchaseOrder = new PurchaseOrderTerm();
    purchaseOrder.sequenceNo = this.purchaseOrder.poTerms.length + 1;
    this.purchaseOrder.poTerms.push(purchaseOrder);
  }

  removePart(data) {
    if (this.orderFormMode === OrderFormMode.Supplier) {
      var index = this.purchaseOrder.poDetails.indexOf(data);
      this.purchaseOrder.poDetails.splice(index, 1);
    }
    else if (this.orderFormMode === OrderFormMode.Customer) {
      var index = this.purchaseOrder.orderDetails.indexOf(data);
      this.purchaseOrder.orderDetails.splice(index, 1);
    }
    
    this.resetSerialNumber();
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
      if (this.validateOrder()) {
        observableResult = this.customerService.savePurchaseOrder(this.purchaseOrder);
      }
    } else {
      if (this.validateOrder()) {
        observableResult = this.supplierService.savePurchaseOrder(this.purchaseOrder);
      }
    }

    this.loaderService.show();
    observableResult.subscribe((result) => {
      this.toastr.successToastr('Details saved successfully.');
      if (this.orderFormMode === OrderFormMode.Customer) {
        if (this.formMode == UserAction.Edit)
          result = this.activatedRoute.snapshot.params.orderId;
        this.uploadDocuments(result);
      }
    }, (error) => {
      this.toastr.errorToastr(error.error);
      this.loaderService.hide();
    }, () => {
      this.loaderService.hide();
      setTimeout(() => {
        if (this.SelectedCustomer > -1)
          this.router.navigateByUrl('customers/purchase-order/0/0');
        else
          this.router.navigateByUrl('suppliers/purchase-order/0/0');
      }, 1000);
    });
  }

  validateOrder(): boolean {
    if (new Date(this.purchaseOrder.dueDate) <= new Date(this.purchaseOrder.poDate)) {
      this.toastr.warningToastr('Due date cannot be less than purchase order date');
      return false;
    }
    if (!this.purchaseOrder.poNo) {
      this.toastr.warningToastr('PO number is mandatory');
      return false;
    }
    if (!this.purchaseOrder.dueDate || !this.purchaseOrder.poDate) {
      this.toastr.warningToastr('PO date & Due date are mandatory');
      return false;
    }
    if (this.orderFormMode === OrderFormMode.Customer && this.purchaseOrder.orderDetails.length < 1) {
      this.toastr.warningToastr('Add at least one part detail to create this order.');
      return false;
    }
    if (this.orderFormMode === OrderFormMode.Supplier && this.purchaseOrder.poDetails.length < 1) {
      this.toastr.warningToastr('Add at least one part detail to create this order.');
      return false;
    }
    return true;
  }

  uploadDocuments(orderNumber: string) {
    this.documents.forEach((item) => {
      this.fileService.uploadFile(item, orderNumber);
      this.toastr.successToastr('File(s) uploaded successfully!!');
    });
  }

  addAttachment(files: FileList, folderName: string) {
    this.documents.push({'type': folderName, 'file': files[0]});
    return;
  }

  dueDateChanged(event) {
    if (this.formMode !== UserAction.Edit && this.orderFormMode === OrderFormMode.Supplier) {
      this.supplierService.getNewPurchaseOrderNumber(this.currentlyLoaddedInCompanyId, this.purchaseOrder.poDate).subscribe(po => 
        this.purchaseOrder.poNo = po.entityNo
      );
    }
  }

  loadDefaultValues() {
    var userSelection = this.activatedRoute.snapshot.params.from;
    switch(userSelection) {
      case "customer":
          this.purchaseOrder.poDate = DateHelper.getToday();
          this.purchaseOrder.dueDate = DateHelper.getTomorrow();
          this.dueDate = DateHelper.getTomorrow();
        break;
      case "supplier":
          this.purchaseOrder.poDate = DateHelper.getToday();
          this.purchaseOrder.dueDate = DateHelper.getTomorrow();
          this.dueDate = DateHelper.getTomorrow();
          this.purchaseOrder.paymentTerms = '75 days from BL';
          this.purchaseOrder.deliveryTerms = 'EX-WORKS';
        break;
      case "all":
        break;
    }
  }
}

enum OrderFormMode {
  Customer = 1,
  Supplier
}