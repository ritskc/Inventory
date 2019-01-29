import { Injectable } from '@angular/core';
import { ConfigService } from '../../config/config.service';
import { ApiService } from '../../common/services/api.service';
import { Product } from '../../models/product.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private config: ConfigService, private apiService: ApiService) { }

  getProduct(id: number): Observable<Product> {
    return this.apiService.get<Product>(this.config.Settings.apiServerHost + this.config.Settings.partsUri + '/' + id);
  }
}
