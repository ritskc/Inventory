import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UploadRoutingModule } from './upload-routing.module';
import { UploadDataComponent } from './upload-data/upload-data.component';
import { SharedModule } from '../../common/shared/shared.module';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../../common/services/api.service';

@NgModule({
  declarations: [
    UploadDataComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    UploadRoutingModule,
    ToastrModule.forRoot()
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
export class UploadModule { }
