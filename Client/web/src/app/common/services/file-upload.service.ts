import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ApiService } from './api.service';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  constructor(private apService: ApiService, private httpClient: HttpClient) { }

  postFile(fileToUpload: File) {
    const formData: FormData = new FormData();
    //formData.append('4', fileToUpload.name);
    formData.append(new Date().getDate().toString(), fileToUpload, fileToUpload.name);
    return this.httpClient.post('http://po.harisons.com/api/File/BL/4', formData)
            .subscribe((result) => console.log(result));
  }

  uploadFile(item: any, filename: string) {
    var formData = new FormData();
      formData.append('file', item.file, item.file.name);
      const req = new HttpRequest('POST', `http://po.harisons.com/api/File/${item.type}/${filename}`, formData, {
        reportProgress: true
      });
      
      const progress = new Subject<number>();

      this.httpClient.request(req).subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {

          // calculate the progress percentage
          const percentDone = Math.round(100 * event.loaded / event.total);

          // pass the percentage into the progress-stream
          progress.next(percentDone);
        } else if (event instanceof HttpResponse) {

          // Close the progress-stream if we get an answer form the API
          // The upload is complete
          progress.complete();
        }
      });
  }
}
