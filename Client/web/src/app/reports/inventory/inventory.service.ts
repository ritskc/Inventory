import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { Part } from '../../models/part.model';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {

  constructor(private apiService: ApiService) { }

  getAllParts() {
    return this.apiService.get<Part[]>('http://po.harisons.com/api/parts');
  }
}
