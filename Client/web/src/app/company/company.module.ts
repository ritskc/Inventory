import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CompanyRoutingModule } from './company-routing.module';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyDetailComponent } from './company-detail/company-detail.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SimpleGridComponent } from '../common/components/simple-grid/simple-grid.component';
import { SearchPipe } from '../common/pipes/search.pipe';
import { PaginatePipe } from '../common/pipes/paginate.pipe';
import { ToastrModule } from 'ng6-toastr-notifications';
import { ReportComponent } from '../common/components/report/report.component';
import { LoaderComponent } from '../common/components/loader/loader.component';
import { httpLoaderService } from '../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../common/services/api.service';
import { SharedModule } from '../common/shared/shared.module';
import { CreateShipmentComponent } from './create-shipment/create-shipment.component';
import { InventoryPartsListComponent } from './inventory-parts-list/inventory-parts-list.component';
import { ShipmentListComponent } from './shipment-list/shipment-list.component';
import { CompanyInvoiceComponent } from './company-invoice/company-invoice.component';
import { POSUploadComponent } from './pos/upload/upload.component';
import { MasterShipmentListComponent } from './master-shipment-list/master-shipment-list.component';
import { MasterShipmentDetailComponent } from './master-shipment-detail/master-shipment-detail.component';
import { SaleReportComponent } from './sale-report/sale-report.component';
import { MonthlyInvoiceListComponent } from './monthly-invoice/monthly-invoice.component';
import { EditMonthlyInvoiceComponent } from './edit-monthly-invoice/edit-monthly-invoice.component';

@NgModule({
  declarations: [
    CompanyListComponent, 
    CompanyDetailComponent,
    CreateShipmentComponent,
    InventoryPartsListComponent,
    ShipmentListComponent,
    CompanyInvoiceComponent,
    POSUploadComponent,
    MasterShipmentListComponent,
    MasterShipmentDetailComponent,
    SaleReportComponent,
    MonthlyInvoiceListComponent,
    EditMonthlyInvoiceComponent
  ],
  imports: [
    CommonModule,
    CompanyRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ToastrModule.forRoot(),
  ],
  providers: [
    httpLoaderService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
  ]
})
export class CompanyModule { }
