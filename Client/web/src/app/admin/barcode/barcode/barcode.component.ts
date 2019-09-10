import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { BarecodeScannerLivestreamComponent } from 'ngx-barcode-scanner';

@Component({
  selector: 'app-barcode',
  templateUrl: './barcode.component.html',
  styleUrls: ['./barcode.component.scss']
})
export class BarcodeComponent implements OnInit, AfterViewInit {

  @ViewChild(BarecodeScannerLivestreamComponent)
  barcodeScanner: BarecodeScannerLivestreamComponent;

  barcodeValue;

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.barcodeScanner.start();
  }

  onValueChanges(result){
    this.barcodeValue = result.codeResult.code;
    alert(this.barcodeValue);
  }
}
