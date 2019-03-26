import { Component, OnInit } from '@angular/core';
import { Part } from '../../../models/part.model';
import { PartsService } from '../parts.service';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CompanyService } from '../../../company/company.service';
import { UserAction } from '../../../models/enum/userAction';

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
}
