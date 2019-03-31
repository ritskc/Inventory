import { Component, OnInit } from '@angular/core';
import { Customer, ShippingInfo } from '../../../models/customer.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '../customer.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Term } from '../../../models/terms.model';
import * as _ from 'lodash';
import { UserAction } from '../../../models/enum/userAction';

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
              private toastr: ToastrManager, private router: Router) { }

  ngOnInit() {
    this.customer = new Customer();
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
      emailAddress: ['', Validators.required]
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
    if (this.customerForm.invalid || !this.verifyIfAValidTermAndConditionExist()) return;

    this.service.saveCustomer(this.customer)
      .subscribe((response) => { 
        this.toastr.successToastr('Details saved successfully.');
      },
      (error) => { 
        this.toastr.errorToastr('Could not save details. Please try again & contact administrator if the problem persists!!')
      }
    );
  }

  removeShippingAddress(index: number) {
    this.customer.shippingInfos.splice(index, 1);
  }

  clearAllValidations() {
    this.submitted = false;
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
