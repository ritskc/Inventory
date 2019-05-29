import { Component, OnInit } from '@angular/core';
import { DataColumn } from '../../../models/dataColumn.model';
import { PartsService } from '../../parts/parts.service';
import { SupplierService } from '../../supplier/supplier.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Supplier } from '../../../models/supplier.model';
import { Part } from '../../../models/part.model';
import { CompanyService } from '../../../company/company.service';
import { Customer } from '../../../models/customer.model';
import { CustomerService } from '../../customer/customer.service';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss']
})
export class OrderDetailComponent implements OnInit {

  private customers: Customer[] = [];
  private suppliers: Supplier[] = [];
  private parts: Part[] = [];
  private filteredParts: Part[] = [];
  private partsGridColumns: DataColumn[] = [];
  private selectedParts: Part[] = [];

  private currentlyLoaddedInCompanyId: number = 0;

  constructor(private partsService: PartsService, private supplierService: SupplierService, private companyService: CompanyService,
    private customerService: CustomerService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoaddedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadSuppliersList();
    this.loadPartsList();
    this.loadCustomersList();
    this.initializePartsGrid();
  }

  initializeFormForSelection() {
    var userSelection = this.activatedRoute.snapshot.params.from;
    switch(userSelection) {
      case "all":
        this.setFormForAllSelection();
        break;
      case "customer":
        this.setFormForCustomerSelection();
        break;
      case "supplier":
        this.setFormForSupplierSelection();
        break;
    }
  }

  initializePartsGrid() {

  }

  loadSuppliersList() {
    this.supplierService.getAllSuppliers(this.currentlyLoaddedInCompanyId)
        .subscribe((suppliers) => this.suppliers = suppliers);
  }

  loadCustomersList() {
    this.customerService.getAllCustomers(this.currentlyLoaddedInCompanyId)
        .subscribe((customers) => this.customers = customers);
  }

  loadPartsList() {
    this.partsService.getAllParts(this.currentlyLoaddedInCompanyId)
        .subscribe((parts) => {
          this.parts = parts;
          this.initializeFormForSelection();
        });
  }

  setFormForAllSelection() {
    this.selectedParts = this.parts;
  }

  setFormForCustomerSelection() {
    alert('setFormForCustomerSelection');
  }

  setFormForSupplierSelection() {
    alert('setFormForSupplierSelection');
  }
}
