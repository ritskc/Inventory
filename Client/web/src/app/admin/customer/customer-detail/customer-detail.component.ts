import { Component, OnInit } from '@angular/core';
import { Customer, ShippingInfo } from '../../../models/customer.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '../customer.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Term } from '../../../models/terms.model';
import * as _ from 'lodash';
import { UserAction } from '../../../models/enum/userAction';
import { CompanyService } from '../../../company/company.service';
import { httpLoaderService } from '../../../common/services/httpLoader.service';

@Component({
  selector: 'app-customer-detail',
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.scss']
})
export class CustomerDetailComponent implements OnInit {

  customer: Customer;
  customerForm: FormGroup;
  submitted: boolean = false;
  atleastOneShippingAddressPresent: boolean = true;

  constructor(private customerBuilder: FormBuilder, private activatedRoute: ActivatedRoute, private service: CustomerService,
              private toastr: ToastrManager, private router: Router, private companyService: CompanyService, private loader: httpLoaderService) { }

  ngOnInit() {
    this.customer = new Customer();
    this.fillDefaultValues();

    if (this.activatedRoute.snapshot.params.action == UserAction.Edit)
      this.getCustomer();

    this.customerForm = this.customerBuilder.group({
      name: ['', Validators.required],
      addressLine1: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zipCode: ['', Validators.required],
      telephoneNumber: ['', Validators.required],
      faxNumber: ['', Validators.required],
      fob: ['', Validators.required],
      terms: ['', Validators.required],
      invoicingtypeid: ['', Validators.required],
      rePackingCharge: ['', Validators.required],
      truckType: ['', Validators.required],
      emailAddress: ['', Validators.required],
      billing: ['', Validators.required],
      contactPersonName: ['', Validators.required],
      collectFreight: ['', Validators.required],
      rePackingPoNo: ['']
    });
  }

  get f() {
    return this.customerForm.controls;
  }

  addMoreShippingAddress() {
    this.customer.shippingInfos.push(new ShippingInfo());
  }

  getCustomer() {
    this.service.getCustomer(this.activatedRoute.snapshot.params.id, this.activatedRoute.snapshot.params.id)
        .subscribe((customer) => this.customer = customer,
                   (error) => console.log(error)); 
  }

  save() {
    this.submitted = true;
    this.customer.companyId = this.companyService.getCurrentlyLoggedInCompanyId();

    if (this.validateForMandatoryFields()) {
      this.loader.show();
      this.service.saveCustomer(this.customer)
        .subscribe((response) => { 
          this.toastr.successToastr('Details saved successfully.');
          setTimeout(() => {
            this.router.navigateByUrl('customers');
          }, 1000);
        },
        (error) => { 
          this.toastr.errorToastr('Could not save details. Please try again & contact administrator if the problem persists!!')
        }, 
        () => this.loader.hide()
      );
    }
  }

  delete() {
    this.service.delete(this.customer.id)
        .subscribe((result) => {
          this.toastr.successToastr("Customer removed successfully.");
          this.router.navigateByUrl("/customers");
        }, (error) => {
          console.log(error);
        });
  }

  removeShippingAddress(index: number) {
    this.customer.shippingInfos.splice(index, 1);
  }

  clearAllValidations() {
    this.submitted = false;
  }

  private fillDefaultValues() {
    if (this.customer.id < 0)
      this.customer.billing = "Account's Payable";
    this.customer.invoicingtypeid = -1;
  }

  private validateForMandatoryFields(): boolean {
    if (this.customer.invoicingtypeid < 0) {
      alert('Please select the invoice type');
      return false;
    }
    return true;
  }

  private verifyIfAValidTermAndConditionExist(): boolean {
    if (this.customer.shippingInfos.length == 0) return false;

    var isValid = true;
    this.atleastOneShippingAddressPresent = isValid;
    this.customer.shippingInfos.forEach(shippingAddress => {
      if (!shippingAddress.addressLine1 && !shippingAddress.city && !shippingAddress.state && !shippingAddress.zipCode) {
        this.atleastOneShippingAddressPresent = isValid = false;
      }
    });
    return isValid;
  }
}
