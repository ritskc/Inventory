import { Component, OnInit } from '@angular/core';
import { DataColumn } from '../../../models/dataColumn.model';
import { Customer } from '../../../models/customer.model';

@Component({
  selector: 'app-upload-data',
  templateUrl: './upload-data.component.html',
  styleUrls: ['./upload-data.component.scss']
})
export class UploadDataComponent implements OnInit {

  private selectedOption: number = -1;
  private data: Customer[] = [];
  private columns: DataColumn[] = [];

  constructor() { }

  ngOnInit() {
  }

  optionSelected() {
    switch (this.selectedOption) {
      case 1:
          this.prepareColumnsForCompanyDataUpload();
        break;
      case 2:
          this.prepareColumnsForCustomerDataUpload();
        break;
      case 3:
        this.prepareColumnsForPartsDataUpload();
        break;
      case 4:
        this.prepareColumnsForSupplierDataUpload();
        break;
    }
  }

  readData() {
    if (this.selectedOption < 0) {
      alert('Please select an option to proceed');
      return;
    }
  }

  prepareColumnsForCompanyDataUpload() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Name", value: "name" }) );
    this.columns.push( new DataColumn({ headerText: "Address", value: "address" }) );
    this.columns.push( new DataColumn({ headerText: "Phone No", value: "phoneNo" }) );
    this.columns.push( new DataColumn({ headerText: "Fax No", value: "faxNo" }) );
    this.columns.push( new DataColumn({ headerText: "Email", value: "eMail" }) );
    this.columns.push( new DataColumn({ headerText: "Contact", value: "contactPersonName" }) );
    this.columns.push( new DataColumn({ headerText: "Warehouse", value: "whName" }) );
    this.columns.push( new DataColumn({ headerText: "Warehose Address", value: "whAddress" }) );
    this.columns.push( new DataColumn({ headerText: "Warehose Contact", value: "whPhoneNo" }) );
    this.columns.push( new DataColumn({ headerText: "Warehose Email", value: "whEmail" }) );
  }

  prepareColumnsForCustomerDataUpload() {

  }

  prepareColumnsForPartsDataUpload() {

  }

  prepareColumnsForSupplierDataUpload() {

  }
}
