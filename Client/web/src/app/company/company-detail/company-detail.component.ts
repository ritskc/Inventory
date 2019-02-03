import { Component, OnInit } from '@angular/core';
import { Company } from '../../models/company.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { UserAction } from '../../models/enum/userAction';
import { CompanyService } from '../company.service';

@Component({
  selector: 'app-company-detail',
  templateUrl: './company-detail.component.html',
  styleUrls: ['./company-detail.component.scss']
})
export class CompanyDetailComponent implements OnInit {

  company: Company
  companyForm: FormGroup;
  submitted: boolean = false;

  constructor(private companyBuilder: FormBuilder, private router: ActivatedRoute, private companyService: CompanyService) { 
    this.company = new Company();

    this.companyForm = this.companyBuilder.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      phoneNo: ['', Validators.required],
      faxNo: ['', Validators.required],
      email: ['', Validators.required],
      contactPersonName: ['', Validators.required]
    });
  }

  ngOnInit() {
    if (this.router.snapshot.params.action == UserAction.Edit)
      this.getCompany();
  }

  get f() {
    return this.companyForm.controls;
  }

  getCompany() {
    this.companyService.getCompany(this.router.snapshot.params.id)
      .subscribe((company) => this.company = company,
    (error) => { console.log(error); });
  }

  save() {
    this.submitted = true;
    if (this.companyForm.invalid) return;

    this.companyService.saveCompany(this.company)
      .subscribe((response) => { console.log(response); },
      (error) => { console.log(error); }
    );
  }
}
