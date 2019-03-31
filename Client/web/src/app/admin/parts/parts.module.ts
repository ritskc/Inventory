import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartsRoutingModule } from './parts-routing.module';
import { PartListComponent } from './part-list/part-list.component';
import { PartDetailComponent } from './part-detail/part-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../common/shared/shared.module';
import { ToastrModule } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../../common/services/api.service';

@NgModule({
  declarations: [
    PartListComponent, 
    PartDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    PartsRoutingModule,
    ReactiveFormsModule,
    SharedModule,
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
export class PartsModule { }
