import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateProductRequest,
  Product,
  ProductLog,
  UpdateProductRequest,
} from './models';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/Products';

  getAll(search?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (search?.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<Product[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, request);
  }

  update(id: number, request: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getLogs(): Observable<ProductLog[]> {
    return this.http.get<ProductLog[]>(`${this.baseUrl}/logs`);
  }

  getLogsByProductId(id: number): Observable<ProductLog[]> {
    return this.http.get<ProductLog[]>(`${this.baseUrl}/${id}/logs`);
  }
}
