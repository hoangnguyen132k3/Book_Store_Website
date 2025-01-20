import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private apiUrl = 'https://localhost:7038/api/Carts';

  constructor(private http: HttpClient) {}

  // Thêm sản phẩm vào giỏ hàng
  addCart(cart: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/add`, cart);
  }
  
  // Lấy toàn bộ giỏ hàng (Tất cả các user - Dành cho admin)
  getAllCarts(): Observable<any> {
    return this.http.get(`${this.apiUrl}/all`);
  }

  // Lấy giỏ hàng theo UserId
  getCartByUserId(userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/${userId}`);
  }

  // Lấy giỏ hàng cụ thể theo UserId và BookId
  getCartByUserAndBook(userId: number, bookId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/${userId}/book/${bookId}`);
  }

  // Cập nhật giỏ hàng (UserId + BookId)
  updateCart(userId: number, bookId: number, cart: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/user/${userId}/book/${bookId}`, cart);
  }

  // Xóa sản phẩm khỏi giỏ hàng (UserId + BookId)
  deleteCart(userId: number, bookId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/user/${userId}/book/${bookId}`);
  }
}
