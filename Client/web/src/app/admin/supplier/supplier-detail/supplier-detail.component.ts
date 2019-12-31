import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SupplierService } from '../supplier.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Supplier } from '../../../models/supplier.model';
import { Term } from '../../../models/terms.model';
import { UserAction } from '../../../models/enum/userAction';
import * as _ from 'lodash';
import { CompanyService } from '../../../company/company.service';

@Component({
  selector: 'app-supplier-detail',
  templateUrl: './supplier-detail.component.html',
  styleUrls: ['./supplier-detail.component.scss']
})
export class SupplierDetailComponent implements OnInit {

  supplier: Supplier;
  supplierForm: FormGroup;
  submitted: boolean = false;
  atleastOneTermPresent: boolean = true;

  constructor(private supplierBuilder: FormBuilder, private activeRoute: ActivatedRoute, private supplierService: SupplierService,
              private toastr: ToastrManager, private router: Router, private companyService: CompanyService) { 
    this.supplier = new Supplier();
    this.supplier.terms = [];

    this.supplierForm = this.supplierBuilder.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      phoneNo: ['', Validators.required],
      faxNo: [''],
      emailID: ['', Validators.required],
      contactPersonName: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      country: ['', Validators.required],
      zipCode: ['', Validators.required],
      dateFormat: ['', Validators.required],
      noofstages: ['', Validators.required]
    });
  }

  ngOnInit() {
    if (this.activeRoute.snapshot.params.action == UserAction.Edit)
      this.getSupplier();
  }

  get f() {
    return this.supplierForm.controls;
  }

  getSupplier() {
    this.supplierService.getSupplier(this.activeRoute.snapshot.params.id, this.activeRoute.snapshot.params.id)
        .subscribe((supplier) => this.supplier = supplier,
                   (error) => { console.log(error) });
  }

  addMoreTermAndCondition() {
    if (this.supplier.terms.length == 0) {
      this.createNewTermAndCondition();
      return;  
    }

    // if (this.verifyIfAValidTermAndConditionExist())
    //   this.createNewTermAndCondition();
  }

  removeTermAndCondition(index) {
    this.supplier.terms.splice(index, 1);
  }
  
  save() {
    this.submitted = true;
    this.supplier.companyId = this.companyService.getCurrentlyLoggedInCompanyId();
    
    if (this.supplierForm.invalid) return;

    this.supplierService.saveSupplier(this.supplier)
      .subscribe((response) => { 
        this.toastr.successToastr('Details saved successfully.');
        this.router.navigateByUrl('/suppliers');
      },
      (error) => { 
        this.toastr.errorToastr('Could not save details. Please try again & contact administrator if the problem persists!!')
      }
    );
  }

  private createNewTermAndCondition() {
    var term = new Term();
    this.supplier.terms.push(term);
  }

  private clearAllValidations() {
    this.submitted = false;
    this.atleastOneTermPresent = true;
  }
  
  cancel() {
    this.router.navigateByUrl('/suppliers');
  }
}
