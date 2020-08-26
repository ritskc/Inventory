import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { BarecodeScannerLivestreamComponent } from 'ngx-barcode-scanner';
import { ToastrManager } from 'ng6-toastr-notifications';
import { DataColumn } from '../../../models/dataColumn.model';
import { ActivatedRoute } from '@angular/router';
import { ShipmentService } from '../../../company/shipment.service';
import { CompanyService } from '../../../company/company.service';
import { Shipment } from '../../../models/shipment.model';
import { httpLoaderService } from '../../../common/services/httpLoader.service';

@Component({
  selector: 'app-barcode',
  templateUrl: './barcode.component.html',
  styleUrls: ['./barcode.component.scss']
})
export class BarcodeComponent implements OnInit, AfterViewInit {

  @ViewChild(BarecodeScannerLivestreamComponent)
  barcodeScanner: BarecodeScannerLivestreamComponent;

  private currentlyLoggedInCompanyId = 0;
  private selection: string = "0";
  private columns: DataColumn[] = [];
  private boxes: any[] = [];
  private shipment: any;
  private isMasterShipment: boolean = false;
  private shipmentNo: string = '';

  barcodeValue;

  constructor(private toastr: ToastrManager, private activatedRoute: ActivatedRoute,
              private shipmentService: ShipmentService, private companyService: CompanyService, private httpLoadService: httpLoaderService) { }

  ngOnInit() {
    this.currentlyLoggedInCompanyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.getShipment(this.activatedRoute.snapshot.params.id);
  }

  ngAfterViewInit() {
    this.barcodeScanner.start();
  }

  onValueChanges(result){
    alert(this.barcodeValue);
    this.barcodeValue = result.codeResult.code;
  }

  getShipment(id) {
    this.columns = [];
    this.boxes = [];

    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "qty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Box", value: "boxeNo", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Barcode", value: "barcode", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Scanned", value: "isScanned", isBoolean: true, isDisabled: true, customStyling: 'center' }) );

    this.httpLoadService.show();
  
    if (window.location.href.indexOf('?type=master') > 0) {
      this.isMasterShipment = true;
      this.shipmentService.getAMasterShipments(this.currentlyLoggedInCompanyId, id)
        .subscribe(shipment => {
          this.shipmentNo = shipment.masterPackingSlipNo;
          this.boxes = shipment.packingSlipBoxDetails;
          this.httpLoadService.hide();
        });
    } else {
      this.shipmentService.getAShipment(this.currentlyLoggedInCompanyId, id)
        .subscribe(shipment => {
          this.shipment = shipment;
          this.shipmentNo = shipment.packingSlipNo;
          shipment.packingSlipDetails.forEach(detail => {
            if (detail.packingSlipBoxDetails.length > 0) {
              detail.packingSlipBoxDetails.forEach(box => {
                box.partCode = detail.partDetail.code;
              })
              this.boxes.push(...detail.packingSlipBoxDetails);
            }
          });
          console.log(this.boxes);
          this.httpLoadService.hide();
        });
    }
  }

  receive() {
    if (!this.barcodeValue) {
      this.toastr.errorToastr('Please scan the barcode');
      return;
    }

    if (this.boxes.length == 0) {
      this.toastr.errorToastr('There are not barcodes to scan');
      return;
    }

    if (this.boxes.filter(b => b.barcode == this.barcodeValue).length == 0) {
      this.toastr.errorToastr('There are no matching barcode in this shipment to scan');
      return;
    }

    this.httpLoadService.show();
    this.shipmentService.completeScanning(this.shipment, this.barcodeValue, window.location.href.indexOf('?type=master') > 0)
        .subscribe(result => {
          console.log(result);
          this.getShipment(this.activatedRoute.snapshot.params.id);
          this.httpLoadService.hide();
          this.toastr.successToastr(`Barcode ${ this.barcodeValue } scanned successfully!!`);
          this.barcodeValue = '';
        }, (error) => {
          this.toastr.errorToastr(error.error);
          this.httpLoadService.hide();
        });

    // switch(this.selection) {
    //   case "0":
    //     alert('Please select the option');
    //     break;
    //   case "1":
    //     var invoiceNumber = this.barcodeValue/1;
    //     this.invoiceService.receivedInvoice(0, invoiceNumber)
    //         .subscribe((result) => this.toastr.successToastr('Received invoice successfully!'));
    //     break;
    //   case "2":
    //       this.invoiceService.receivedBox(this.barcodeValue)
    //         .subscribe((result) => this.toastr.successToastr('Received box successfully!'));
    //     break;
    // }
  }
}
