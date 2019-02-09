import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient) { }

  get<T>(url: string) {
    return this.http.get<T>(url);
  }

  post<T>(t: T, url: string) {
    return this.http.post(url, t);
  }

  put<T>(t: T, url: string) {
    return this.http.put(url, t);
  }

  delete(id: number, url: string) {
    return this.http.delete(`${ url }/${ id }`);
  }
}
