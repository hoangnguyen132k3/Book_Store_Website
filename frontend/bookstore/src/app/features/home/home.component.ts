import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../core/component/footer/footer.component';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
})
export class HomeComponent implements OnInit {
  categories: any[] = []; // Danh mục
  flashSales: any[] = []; // Danh sách sản phẩm
  isLoading = true; // Trạng thái tải
  errorMessage = ''; // Lỗi

  constructor(
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (data) => {
        this.categories = data.map((category: any) => ({
          id: category.categoryId,
          name: category.name,
          image: category.imageUrl,
        }));
      },
      error: (err) => {
        console.error('Không thể tải danh mục:', err);
        this.errorMessage = 'Không thể tải danh mục. Vui lòng thử lại sau!';
      },
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.flashSales = data.map((product: any) => ({
          id: product.bookId,
          name: product.name,
          price: product.price,
          originalPrice: product.oldPrice,
          imageUrl: product.imageUrl,
        }));
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Không thể tải danh sách sản phẩm:', err);
        this.errorMessage = 'Không thể tải danh sách sản phẩm. Vui lòng thử lại sau!';
        this.isLoading = false;
      },
    });
  }

  onCategoryClick(categoryId: number): void {
    this.isLoading = true;
    this.flashSales = [];

    this.productService.getBooksByCategory(categoryId).subscribe({
      next: (data) => {
        this.flashSales = data.map((product: any) => ({
          id: product.bookId,
          name: product.name,
          price: product.price,
          originalPrice: product.oldPrice,
          imageUrl: product.imageUrl,
        }));
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Không thể tải sách theo danh mục:', err);
        this.errorMessage = 'Không thể tải sách theo danh mục. Vui lòng thử lại sau!';
        this.isLoading = false;
      },
    });
  }

  proceedToProductDetail(productId: number): void {
    if (productId) {
      this.router.navigate(['/product-detail', productId]);
    } else {
      console.error('Product ID is invalid');
    }
  }

  addToCart(productId: number): void {
    if (this.authService.isLoggedIn()) {
      const currentTime = new Date().toISOString(); // Lấy thời gian hiện tại
      const cartItem = {
        userId: this.authService.getCurrentUserId(), // Lấy ID người dùng
        bookId: productId,
        quantity: 1, // Mặc định số lượng
        addedAt: currentTime, // Thời gian thêm
        totalPrice: 0, // Tổng giá (nếu cần, có thể được tính ở phía server)
        status: 'Added', // Trạng thái mặc định
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
