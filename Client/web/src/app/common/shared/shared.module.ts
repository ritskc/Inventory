import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginatePipe } from '../pipes/paginate.pipe';
import { SimpleGridComponent } from '../components/simple-grid/simple-grid.component';
import { SearchPipe } from '../pipes/search.pipe';
import { LoaderComponent } from '../components/loader/loader.component';
import { FormsModule } from '@angular/forms';
import { SafePipe } from '../pipes/safe.pipe';

@NgModule({
  declarations: [
    LoaderComponent,
    PaginatePipe,
    SearchPipe,
    SafePipe,
    SimpleGridComponent,
  ],
  exports: [
    LoaderComponent,
    PaginatePipe,
    SafePipe,
    SearchPipe,
    SimpleGridComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ]
})
export class SharedModule { }
