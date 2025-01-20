import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:7038/api/Authentication';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login-user`, { email, password });
  }

  saveToken(token: string, expiresAt: string): void {
    if (token && expiresAt) {
      localStorage.setItem('token', token);
      localStorage.setItem('expiresAt', expiresAt);
    }
  }

  saveUser(user: any): void {
    if (user && user.userId) {
      const userData = {
        id: user.userId, // Lưu `userId` từ server
        email: user.email, // Lưu thêm email nếu cần
      };
      localStorage.setItem('user', JSON.stringify(userData));
    }
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    const expiresAt = localStorage.getItem('expiresAt');
    return !!token && !!expiresAt && new Date(expiresAt).getTime() > new Date().getTime();
  }

  getCurrentUserId(): number {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    return user?.id || 0; // Truy xuất `userId` từ localStorage
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('expiresAt');
    localStorage.removeItem('user'); // Xóa cả thông tin user khi đăng xuất
  }

  registerUser(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register-user`, data);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
