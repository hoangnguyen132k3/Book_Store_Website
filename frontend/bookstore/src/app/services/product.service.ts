import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  // URL API cho sách
  private booksApiUrl = 'https://localhost:7038/api/Books';

  // URL API cho danh mục
  private categoriesApiUrl = 'https://localhost:7038/api/Categories';

  constructor(private http: HttpClient) {}

  // Lấy danh sách tất cả sản phẩm
  getProducts(): Observable<any> {
    return this.http.get(`${this.booksApiUrl}/get-all-books`);
  }

  // Lấy thông tin chi tiết sản phẩm theo ID
  getProductById(id: number): Observable<any> {
    return this.http.get(`${this.booksApiUrl}/get-book-by-id/${id}`);
  }

  getBooksByCategory(categoryId: number): Observable<any> {
    return this.http.get(`${this.booksApiUrl}/get-book-by-category/${categoryId}`);
  }
  
  
  getCategories(): Observable<any> {
    const headers = { Authorization: 'this-is-just-a-secret-key-here-abc' };
    return this.http.get('https://localhost:7038/api/Categores/get-all-categories', { headers });
  }

  searchProductsByName(name: string): Observable<any[]> {
    const apiUrl = `https://localhost:7038/api/Books/search-by-name?name=${encodeURIComponent(name)}`;
    return this.http.get<any[]>(apiUrl);
  }
  
  
  
}
