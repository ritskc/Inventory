import { Component, OnInit } from '@angular/core';
import { Part, PartCustomerAssignment, PartSupplierAssignment } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CompanyService } from '../../../company/company.service';
import { UserAction } from '../../../models/enum/userAction';
import { DataColumn } from '../../../models/dataColumn.model';
import { Customer } from '../../../models/customer.model';
import { CustomerService } from '../../customer/customer.service';
import { Supplier } from '../../../models/supplier.model';
import { SupplierService } from '../../supplier/supplier.service';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-part-detail',
  templateUrl: './part-detail.component.html',
  styleUrls: ['./part-detail.component.scss']
})
export class PartDetailComponent implements OnInit {

  part: Part;
  partForm: FormGroup;
  submitted: boolean = false;
  atleastOneSupplierPresent: boolean = false;
  atleastOneCustomerPresent: boolean = false;
  currentlyLoggedInCompanyId: number = 0;
  customers: Customer[] = [];
  suppliers: Supplier[] = [];

  constructor(private formBuilder: FormBuilder, private service: PartsService, private activatedRoute: ActivatedRoute,
              private companyService: CompanyService, private customerService: CustomerService,
              private supplierService: SupplierService, private toastr: ToastrManager, private router: Router) {

    this.part = new Part();
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.part.companyId = this.currentlyLoggedInCompanyId;

    this.partForm = this.formBuilder.group({
      code: ['', Validators.required],
      description: ['', Validators.required],
      weightInKg: ['', Validators.required],
      weightInLb: ['', Validators.required],
      openingQty: ['', Validators.required],
      minQty: ['', Validators.required],
      maxQty: ['', Validators.required],
      drawingNo: ['', Validators.required]
    })
  }

  ngOnInit() {
    this.customerService.getAllCustomers(this.currentlyLoggedInCompanyId)
        .subscribe((customers) => this.customers = customers,
                   (error) => console.log(error));

    this.supplierService.getAllSuppliers(this.currentlyLoggedInCompanyId)
        .subscribe((suppliers) => this.suppliers = suppliers, 
                   (error) => console.log(error));

    if (this.activatedRoute.snapshot.params.action == UserAction.Edit)
      this.getPart();
  }

  f() {
    return this.partForm.controls;
  }

  getPart() {
    this.service.getPart(this.currentlyLoggedInCompanyId, this.activatedRoute.snapshot.params.id)
        .subscribe(
          (part) => this.part = part,
          (error) => console.log(error)
        );
  }

  addSupplierInfo() {
    if (!this.part.code || !this.part.description) {
      alert('Part code and description are mandatory to proceed.');
      return;
    }
    var partToAdd = new PartSupplierAssignment();
    partToAdd.mapCode = this.part.code;
    this.part.partSupplierAssignments.push(partToAdd);
  }

  removeSupplierAssignment(index) {
    this.part.partSupplierAssignments.splice(index, 1);
  }

  addCustomerInfo() {
    if (!this.part.code || !this.part.description) {
      alert('Part code and description are mandatory to proceed.');
      return;
    }
    var partToAdd = new PartCustomerAssignment();
    partToAdd.mapCode = this.part.code;
    this.part.partCustomerAssignments.push(partToAdd);
  }

  removeCustomerAssignment(index) {
    this.part.partCustomerAssignments.splice(index, 1);
  }

  clearAllValidations() {
    this.submitted = false;
  }

  save() {
    if (!this.validatePart()) 
      return;

    this.service.save(this.part)
        .subscribe((result) => {
          this.toastr.successToastr('Details saved successfully.');
          this.router.navigateByUrl('/parts');
        },
        (error) => {
          this.toastr.errorToastr(error.error);
        });
  }

  delete() {
    this.service.delete(this.part.id)
        .subscribe((result) => {
          this.toastr.successToastr('Part deleted successfully.');
          this.router.navigateByUrl('/parts');
         }, (error) => { console.log(error) });
  }

  validatePart(): boolean {
    if (this.part.partCustomerAssignments.some(c => !c.mapCode)) {
      this.toastr.errorToastr('Not all parts added to customer have map codes. Please verify.');
      return false;
    }
    if (this.part.partSupplierAssignments.some(s => !s.mapCode)) {
      this.toastr.errorToastr('Not all parts added to supplier have map codes. Please verify.');
      return false;
    }

    if (this.part.partCustomerAssignments.some(c => c.customerId < 1)) {
      this.toastr.errorToastr('Some of the customer association are missing. Please verify.');
      return false;
    }
    if (this.part.partSupplierAssignments.some(s => s.supplierID < 1)) {
      this.toastr.errorToastr('Some of the customer association are missing. Please verify.');
      return false;
    }

    if (this.part.partCustomerAssignments.some(c => c.rate < 0)) {
      this.toastr.errorToastr('Some of the rates in customer association are invalid. Please verify.');
      return false;
    }
    if (this.part.partCustomerAssignments.some(c => c.surchargePerPound < 0)) {
      this.toastr.errorToastr('Some of the surcharge in customer association are invalid. Please verify.');
      return false;
    }
    if (this.part.partSupplierAssignments.some(s => s.unitPrice < 0)) {
      this.toastr.errorToastr('Some of the unit prices in supplier association are invalid. Please verify.');
      return false;
    }

    return true;
  }
}