import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { FooterComponent } from '../../../../core/component/footer/footer.component';
import { ProductService } from '../../../../services/product.service';
import { CartService } from '../../../../services/cart.service';
import { AuthService } from '../../../../services/auth.service';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';

@Component({
  standalone: true,
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css'],
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
})
export class CategoryComponent implements OnInit {
  categories: any[] = []; // Danh mục
  flashSales: any[] = []; // Danh sách sản phẩm
  isLoading = true; // Trạng thái tải
  errorMessage = ''; // Lỗi
  selectedCategoryId: number | null = null; // Lưu danh mục được chọn

  constructor(
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts(); // Tải toàn bộ sản phẩm ban đầu
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (data: any) => {
        this.categories = data.map((category: any) => ({
          id: category.categoryId,
          name: category.name,
          image: category.imageUrl,
        }));
      },
      error: (err: any) => {
        console.error('Không thể tải danh mục:', err);
        this.errorMessage = 'Không thể tải danh mục. Vui lòng thử lại sau!';
      },
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (data: any) => {
        this.flashSales = data.map((product: any) => ({
          id: product.bookId,
          name: product.name,
          price: product.price,
          originalPrice: product.oldPrice,
          imageUrl: product.imageUrl,
        }));
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Không thể tải danh sách sản phẩm:', err);
        this.errorMessage = 'Không thể tải danh sách sản phẩm. Vui lòng thử lại sau!';
        this.isLoading = false;
      },
    });
  }

  onCategoryClick(categoryId: number): void {
    this.isLoading = true;
    this.selectedCategoryId = categoryId;

    this.productService.getBooksByCategory(categoryId).subscribe({
      next: (data: any) => {
        this.flashSales = data.map((product: any) => ({
          id: product.bookId,
          name: product.name,
          price: product.price,
          originalPrice: product.oldPrice,
          imageUrl: product.imageUrl,
        }));
        this.isLoading = false;
      },
      error: (err: any) => {
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
        error: (err: any) => {
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
