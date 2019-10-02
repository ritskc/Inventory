import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  constructor(private apService: ApiService, private httpClient: HttpClient) { }

  postFile(fileToUpload: File) {
    const formData: FormData = new FormData();
    formData.append(new Date().getDate().toString(), fileToUpload, fileToUpload.name);
    return this.httpClient.post('https://questapi.yellow-chips.com/File/bl/2', formData)
            .subscribe((result) => console.log(result));
  }
}
