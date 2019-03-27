import { Component, OnInit } from '@angular/core';
import { Part } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CompanyService } from '../../../company/company.service';
import { UserAction } from '../../../models/enum/userAction';
import { DataColumn } from '../../../models/dataColumn.model';

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
  customerGridDataColumns: DataColumn[] = [];
  supplierGridDataColumns: DataColumn[] = [];

  constructor(private formBuilder: FormBuilder, private service: PartsService, private activatedRoute: ActivatedRoute,
              private companyService: CompanyService) {
    this.part = new Part();
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();

    this.partForm = this.formBuilder.group({
      code: ['', Validators.required],
      description: ['', Validators.required],
      weightInKg: ['', Validators.required, Validators.min(0), Validators.max(1000)],
      weightInLb: ['', Validators.required, Validators.min(0), Validators.max(3000)],
      openingQty: ['', Validators.required, Validators.min(0), Validators.max(1000)],
      minQty: ['', Validators.required, Validators.min(0), Validators.max(1000)],
      maxQty: ['', Validators.required, Validators.min(0), Validators.max(1000)],
      drawingNo: ['', Validators.required, Validators.min(0), Validators.max(1000)]
    })
  }

  ngOnInit() {
    this.getColumnsForDataSelection();
    if (this.activatedRoute.snapshot.params.action == UserAction.Edit)
      this.getPart();
  }

  getColumnsForDataSelection() {
    this.customerGridDataColumns.push( new DataColumn({ headerText: "Customer", value: "name" }) );
    this.customerGridDataColumns.push( new DataColumn({ headerText: "Map Code", value: "addressLine1" }) );
    this.customerGridDataColumns.push( new DataColumn({ headerText: "Price", value: "telephoneNumber" }) );
    this.customerGridDataColumns.push( new DataColumn({ headerText: "Surcharge", value: "emailAddress" }) );
    this.customerGridDataColumns.push( new DataColumn({ headerText: "Surcharge Fee", value: "emailAddress" }) );

    this.supplierGridDataColumns.push( new DataColumn({ headerText: "Supplier", value: "name" }) );
    this.supplierGridDataColumns.push( new DataColumn({ headerText: "Map Code", value: "addressLine1" }) );
    this.supplierGridDataColumns.push( new DataColumn({ headerText: "Price", value: "telephoneNumber" }) );
    this.supplierGridDataColumns.push( new DataColumn({ headerText: "Hide on Update App", value: "emailAddress" }) );
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

  clearAllValidations() {
    this.submitted = false;
  }

  addCustomer() {

  }

  addSupplier() {

  }
}
