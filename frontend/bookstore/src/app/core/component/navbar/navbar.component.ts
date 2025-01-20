import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
})
export class NavbarComponent {
  searchTerm: string = ''; 
  isLoggedIn = false; 
  showUserMenu = false; 
  searchResults: any[] = []; 

  private searchApiUrl = 'https://localhost:7038/api/Books/search-by-name'; 

  constructor(
    private router: Router,
    private authService: AuthService,
    private http: HttpClient 
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  search(): void {
    if (this.searchTerm.trim()) {
      this.http
        .get(`${this.searchApiUrl}?name=${encodeURIComponent(this.searchTerm)}`)
        .subscribe({
          next: (data: any) => {
            this.searchResults = data;
            console.log('Kết quả tìm kiếm:', this.searchResults);
            this.router.navigate(['/search'], {
              queryParams: { q: this.searchTerm },
              state: { results: this.searchResults }, 
            });
          },
          error: (err) => {
            console.error('Lỗi khi tìm kiếm:', err);
          },
        });
    }
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
  }

  goToPage(path: string): void {
    this.router.navigate([path]);
    this.closeMenu();
  }

  logout(): void {
    this.authService.logout(); 
    this.isLoggedIn = false; 
    this.router.navigate(['/login']); 
    this.closeMenu();
  }


  closeMenu(): void {
    this.showUserMenu = false;
  }
}
