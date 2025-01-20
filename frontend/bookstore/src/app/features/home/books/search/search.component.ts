import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../../../core/component/footer/footer.component';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../../services/product.service';
import { CartService } from '../../../../services/cart.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
})
export class SearchComponent implements OnInit {
  searchTerm: string = ''; 
  flashSales: any[] = []; 
  isLoading = true; 
  errorMessage = ''; 

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.searchTerm = params['q'] || '';
      if (this.searchTerm) {
        this.searchProducts(this.searchTerm);
      } else {
        this.isLoading = false;
        this.errorMessage = 'Vui lòng nhập từ khóa tìm kiếm.';
      }
    });
  }

  searchProducts(term: string): void {
    this.productService.searchProductsByName(term).subscribe({
      next: (data) => {
        this.flashSales = data.map((product: any) => ({
          id: product.bookId,
          name: product.name,
          price: product.price,
          originalPrice: product.oldPrice,
          image: product.imageUrl,
        }));
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Lỗi khi tìm kiếm sản phẩm:', err);
        this.errorMessage = 'Không thể tải kết quả tìm kiếm. Vui lòng thử lại sau!';
        this.isLoading = false;
      },
    });
  }

  proceedToProductDetail(productId: number): void {
    this.router.navigate(['/product-detail', productId]);
  }

  addToCart(productId: number): void {
    if (this.authService.isLoggedIn()) {
      const currentTime = new Date().toISOString(); 
      const cartItem = {
        userId: this.authService.getCurrentUserId(), 
        bookId: productId,
        quantity: 1, 
        addedAt: currentTime,
        totalPrice: 0, 
        status: 'Added',
      };

      this.cartService.addCart(cartItem).subscribe({
        next: () => {
          console.log('Sản phẩm đã được thêm vào giỏ hàng:', cartItem);
          alert('Sản phẩm đã được thêm vào giỏ hàng!');
        },
        error: (err) => {
          console.error('Không thể thêm sản phẩm vào giỏ hàng:', err);
          alert('Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại sau!');
        },
      });
    } else {
      alert('Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng.');
      this.router.navigate(['/login']);
    }
  }
}
