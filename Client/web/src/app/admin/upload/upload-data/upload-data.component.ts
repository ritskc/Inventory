import { Component, OnInit } from '@angular/core';
import { DataColumn } from '../../../models/dataColumn.model';
import { Customer } from '../../../models/customer.model';
import { ToastrManager } from 'ng6-toastr-notifications';
import { CompanyService } from '../../../company/company.service';
import { Company } from '../../../models/company.model';
import readXlsxFile from 'read-excel-file';
import { Part } from '../../../models/part.model';
import { PartsService } from '../../parts/parts.service';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-upload-data',
  templateUrl: './upload-data.component.html',
  styleUrls: ['./upload-data.component.scss']
})
export class UploadDataComponent implements OnInit {

  private selectedOption: number = -1;
  private dataToDisplay: any[] = [];
  private data: any[] = [];
  private invalidData: any[] = [];
  private columns: DataColumn[] = [];
  private company: Company;
  private parts: Part[] = [];
  private dataValidated: any;
  private filterInvalidItems: boolean = false;

  constructor(private toastr: ToastrManager, private companySerive: CompanyService, private partsService: PartsService,
              private loaderService: httpLoaderService) { }

  ngOnInit() {
    var companyId = this.companySerive.getCurrentlyLoggedInCompanyId();
    this.companySerive.getCompany(companyId)
        .subscribe(company => this.company = company);
  }

  optionSelected(rows: any) {
    switch (this.selectedOption.toString()) {
      case '1':
          this.prepareColumnsForCompanyDataUpload(rows);
        break;
    }
  }

  readData(files: FileList) {
    if (this.selectedOption < 0) {
      this.toastr.warningToastr('Please select an option to proceed');
      return;
    }
    
    readXlsxFile(files[0]).then((rows) => {
      this.optionSelected(rows);
    });
  }

  prepareColumnsForCompanyDataUpload(rows: any) {
    this.columns = [];
    this.data = [];
    this.columns.push( new DataColumn({ headerText: "Company", value: "companyName" }) );
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode" }) );
    this.columns.push( new DataColumn({ headerText: "Part Description", value: "partDescription" }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "openingQty", customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Valid", value: "valid", isBoolean: true, isDisabled: true, customStyling: 'center' }) );

    for (let index = 1; index < rows.length; index++) {
      var viewModel = new OpeningQuantityViewModel();
      viewModel.companyName = this.company.name;
      viewModel.partCode = rows[index][0];
      viewModel.openingQty = rows[index][1];
      viewModel.valid = false;
      this.data.push(viewModel);
    }
    this.dataValidated = false;
    this.dataToDisplay = this.data;
  }

  validateData() {
    switch(this.selectedOption.toString()) {
      case '1':
        this.validateParts();
        break;
    }
  }

  uploadBulkData() {
    switch(this.selectedOption.toString()) {
      case '1':
        this.uploadOpeningQuantities();
        break;
    }
  }

  private validateParts() {
    this.invalidData = [];
    this.loaderService.show();
    this.partsService.getAllParts(this.company.id)
        .subscribe(parts => {
          this.parts = parts;
          this.data.forEach(item => {
            var filteredPart = parts.find(p => p.code === item.partCode);
            if (filteredPart) {
              item.valid = true;
              item.partDescription = filteredPart.description;
            } else {
              this.invalidData.push(item);
            }
          });

          if (this.data.some(d => d.valid === false)) {
            this.dataValidated = false;
            this.toastr.warningToastr('Not all parts are valid. Please check the entries to proceed further.');
          } else {
            this.dataValidated = true;
          }
          this.dataToDisplay = this.data;
        }, (error) => { this.toastr.errorToastr(error.error) },
        () => this.loaderService.hide());
  }

  private filterInvalidItemsEvent() {
    this.dataToDisplay = this.filterInvalidItems? this.invalidData: this.data;
  }

  private resetScreen() {
    this.data = [];
    this.invalidData = [];
    this.dataToDisplay = [];
    this.dataValidated = false;
  }

  private uploadOpeningQuantities() {
    var updatedItemsCount = 0;
    this.loaderService.show();
    this.data.forEach(item => {
      var part = this.parts.find(p => p.code === item.partCode);
      if (part) {
        this.partsService.updateOpeningQuantityByPartCode(part, this.company.id, item.partCode, item.openingQty)
            .subscribe(() => {}, (error) => this.toastr.errorToastr(error.error), 
            () => { 
              updatedItemsCount += 1;
              if (updatedItemsCount == this.data.length) {
                this.toastr.successToastr('Opening quantity updated for all parts successfully!!');
                this.loaderService.hide();
                setTimeout(() => {
                  this.resetScreen();
                }, 1500);
              }
            });
      }
      
    });
  }
}

class OpeningQuantityViewModel {
  companyName: string;
  partCode: string;
  partDescription: string;
  openingQty: number = 0;
  valid: boolean;
}