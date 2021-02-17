import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DataColumn } from '../../models/dataColumn.model';
import { Customer } from '../../models/customer.model';
import { Shipment, PackingSlipDetail } from '../../models/shipment.model';
import { CustomerService } from '../../admin/customer/customer.service';
import { ShipmentService } from '../shipment.service';
import { PartsService } from '../../admin/parts/parts.service';
import { Part } from '../../models/part.model';
import { InvoiceService } from '../../admin/invoice/invoice.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Subject } from 'rxjs';
import { AppConfigurations } from '../../config/app.config';
import { httpLoaderService } from '../../common/services/httpLoader.service';

@Component({
  selector: 'app-company-invoice',
  templateUrl: './company-invoice.component.html',
  styleUrls: ['./company-invoice.component.scss']
})
export class CompanyInvoiceComponent implements OnInit {

  private customerId: number = -1;
  private shipmentId: number = -1;
  private parts: Part[] = [];
  private customers: Customer[] = [];
  private shipments: Shipment[];
  private selectedShipment: Shipment = new Shipment();
  private currentlyLoggedInCompany: number = 0;
  private invoiceCreated: Subject<string> = new Subject<string>();
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private customerService: CustomerService, private invoiceService: InvoiceService, private loaderService: httpLoaderService,
              private shipmentService: ShipmentService, private partsService: PartsService, private toastr: ToastrManager, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
    this.loadAllParts();
    this.initializeGridColumns();
  }

  loadAllCustomers() {
    this.loaderService.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe((customers) => {
          this.customers = customers;
          this.loaderService.hide();
          if (this.activatedRoute.snapshot.params.customerId) {
            this.customerId = this.activatedRoute.snapshot.params.customerId;
            this.customeSelected();
          }
        });
  }

  loadAllParts() {
    this.loaderService.show();
    this.partsService.getAllParts(this.currentlyLoggedInCompany)
        .subscribe((parts) => {
          this.parts = parts;
          this.loaderService.hide();
        });
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "partDescription", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "PO No", value: "orderNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Qty", value: "qty", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", sortable: false, isEditable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Total", value: "total", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "In Basket", value: "inBasket", sortable: false, isBoolean: true, customStyling: 'center' }) );
    this.columns.push( new DataColumn({ headerText: "Surcharge", value: "surcharge", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Sur/Pound", value: "surchargePerPound", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Sur/Unit", value: "surchargePerUnit", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Sur/Pound", value: "surchargePerPound", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Total Sur", value: "totalSurcharge", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Excess Qty", value: "excessQty", sortable: false, customStyling: 'right' }) );
  }

  customeSelected() {
    this.selectedShipment = new Shipment();
    this.loaderService.show();
    this.shipmentService.getAllShipments(this.currentlyLoggedInCompany)
        .subscribe((shipments) => {
          if (this.activatedRoute.snapshot.params.shipmentId) {
            this.shipmentId = this.activatedRoute.snapshot.params.shipmentId;
            this.shipments = shipments.filter(s => s.customerId == this.customerId && s.id == this.shipmentId);
            this.shipmentSelected();
          } else {
            this.shipments = shipments.filter(s => s.customerId == this.customerId && !s.isInvoiceCreated);
          }
          this.loaderService.hide();
        });
  }

  shipmentSelected() {
    this.selectedShipment = new Shipment();
    this.selectedShipment = this.shipments.find(s => s.id == this.shipmentId);
    this.selectedShipment.packingSlipDetails.forEach(detail => {
      detail.price = detail.qty * detail.unitPrice;
      var part = this.parts.find(p => p.id == detail.partId);
      detail.partCode = part.code;
      detail.partDescription = part.description;
      detail.hasPartQuantityExceedingLimit = part.isDoublePricingAllowed && detail.qty > part.currentPricingInEffectQty;
    });
  }

  unitPriceChanged(row) {
    row.row.total = row.row.qty * row.row.unitPrice;
  }

  createInvoice() {
    var hasPartsExceedingLimit = '';
    this.selectedShipment.packingSlipDetails.forEach(detail => {
      if (detail.hasPartQuantityExceedingLimit)
        hasPartsExceedingLimit += detail.partDescription + ', ';
    });

    if (hasPartsExceedingLimit) {
      if (!confirm(`The parts ${ hasPartsExceedingLimit } has quantities which might change the price. Are you sure to continue?`))
        return;
    }

    this.loaderService.show();
    this.invoiceService.createCustomerInvoice(this.selectedShipment)
        .subscribe((result) => {
          let appConfig = new AppConfigurations();
          this.invoiceCreated.next(`${appConfig.reportsUri}/Invoice.aspx?id=${this.selectedShipment.id}`);
          this.toastr.successToastr('Updated details successfully!!');
          this.customeSelected();
          this.loaderService.hide();
        })
  }
}