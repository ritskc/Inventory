import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UploadDataComponent } from './upload-data/upload-data.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Upload Data'
  }, 
  children: [{
    path: '',
    component: UploadDataComponent
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UploadRoutingModule { }
