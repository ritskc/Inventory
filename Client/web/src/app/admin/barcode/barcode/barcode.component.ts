import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { BarecodeScannerLivestreamComponent } from 'ngx-barcode-scanner';
import { InvoiceService } from '../../invoice/invoice.service';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-barcode',
  templateUrl: './barcode.component.html',
  styleUrls: ['./barcode.component.scss']
})
export class BarcodeComponent implements OnInit, AfterViewInit {

  @ViewChild(BarecodeScannerLivestreamComponent)
  barcodeScanner: BarecodeScannerLivestreamComponent;

  private selection: string = "0";
  barcodeValue;

  constructor(private invoiceService: InvoiceService, private toastr: ToastrManager) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.barcodeScanner.start();
  }

  onValueChanges(result){
    alert(this.barcodeValue);
    this.barcodeValue = result.codeResult.code;
  }

  receive() {
    if (!this.barcodeValue) {
      alert('Please scan the barcode');
      return;
    }

    switch(this.selection) {
      case "0":
        alert('Please select the option');
        break;
      case "1":
        var invoiceNumber = this.barcodeValue/1;
        this.invoiceService.receivedInvoice(0, invoiceNumber)
            .subscribe((result) => this.toastr.successToastr('Received invoice successfully!'));
        break;
      case "2":
          this.invoiceService.receivedBox(this.barcodeValue)
            .subscribe((result) => this.toastr.successToastr('Received box successfully!'));
        break;
    }
  }
}
