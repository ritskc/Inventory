import { Component, OnInit } from '@angular/core';
import { Company, Warehouse } from '../../models/company.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserAction } from '../../models/enum/userAction';
import { CompanyService } from '../company.service';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-company-detail',
  templateUrl: './company-detail.component.html',
  styleUrls: ['./company-detail.component.scss']
})
export class CompanyDetailComponent implements OnInit {

  company: Company
  companyForm: FormGroup;
  submitted: boolean = false;

  constructor(private companyBuilder: FormBuilder, private activeRouter: ActivatedRoute, 
    private companyService: CompanyService, private toastr: ToastrManager, private router: Router) { 
    this.company = new Company();

    this.companyForm = this.companyBuilder.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      phoneNo: ['', Validators.required],
      faxNo: ['', Validators.required],
      email: ['', Validators.required],
      contactPersonName: ['', Validators.required],
      whName: ['', Validators.required],
      whAddress: ['', Validators.required],
      whPhoneNo: ['', Validators.required],
      whEmail: ['', Validators.required]
    });
  }

  ngOnInit() {
    if (this.activeRouter.snapshot.params.action == UserAction.Edit)
      this.getCompany();
  }

  get f() {
    return this.companyForm.controls;
  }

  getCompany() {
    this.companyService.getCompany(this.activeRouter.snapshot.params.id)
      .subscribe((company) => this.company = company,
    (error) => { console.log(error); });
  }

  save() {
    this.submitted = true;
    if (this.companyForm.invalid) return;

    if (this.company.warehouses.filter(w => w.name == '').length > 0) {
      this.toastr.errorToastr('Warehouse name cannot be empty');
      return;
    }

    this.companyService.saveCompany(this.company)
      .subscribe((response) => { 
        this.toastr.successToastr('Details saved successfully.');
      },
      (error) => { 
        this.toastr.errorToastr('Could not save details. Please try again & contact administrator if the problem persists!!')
      }
    );
  }

  delete() {
    this.companyService.deleteCompany(this.company.id)
      .subscribe(
        (response) => {
          this.toastr.successToastr('Company deleted successfully');
          this.router.navigateByUrl(`/companies`);
        },
      (error) => {
        this.toastr.errorToastr('Could not delete the company. Please try again & contact administrator if the problem persists.');
      })
  }

  addMoreWarehouse() {
    this.company.warehouses.push(new Warehouse());
  }

  clear() {
    this.company = new Company();
  }
}
