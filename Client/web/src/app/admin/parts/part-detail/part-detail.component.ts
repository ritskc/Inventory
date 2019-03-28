import { Component, OnInit } from '@angular/core';
import { Part } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CompanyService } from '../../../company/company.service';
import { UserAction } from '../../../models/enum/userAction';
import { DataColumn } from '../../../models/dataColumn.model';
import { Customer } from '../../../models/customer.model';
import { CustomerService } from '../../customer/customer.service';

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

  constructor(private formBuilder: FormBuilder, private service: PartsService, private activatedRoute: ActivatedRoute,
              private companyService: CompanyService, private customerService: CustomerService) {
    this.part = new Part();
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();

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

  clearAllValidations() {
    this.submitted = false;
  }

  addCustomer() {

  }

  addSupplier() {

  }
}
