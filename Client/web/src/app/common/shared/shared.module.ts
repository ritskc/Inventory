import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginatePipe } from '../pipes/paginate.pipe';
import { SimpleGridComponent } from '../components/simple-grid/simple-grid.component';
import { SearchPipe } from '../pipes/search.pipe';
import { LoaderComponent } from '../components/loader/loader.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    LoaderComponent,
    PaginatePipe,
    SearchPipe,
    SimpleGridComponent,
  ],
  exports: [
    LoaderComponent,
    PaginatePipe,
    SearchPipe,
    SimpleGridComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ]
})
export class SharedModule { }
