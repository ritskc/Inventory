import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UploadRoutingModule } from './upload-routing.module';
import { UploadDataComponent } from './upload-data/upload-data.component';
import { SharedModule } from '../../common/shared/shared.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    UploadDataComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    UploadRoutingModule
  ]
})
export class UploadModule { }
