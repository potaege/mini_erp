export interface AuthSession {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
}

export interface LoginRequest {
  name: string;
  password: string;
}

export interface Product {
  id: number;
  name: string;
  sku: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  price: number;
  stockQuantity: number;
}

export interface UpdateProductRequest {
  name: string;
  sku: string;
  price: number;
}

export interface ProductLog {
  id: number;
  productId: number;
  productName: string | null;
  productSku: string | null;
  userId: number;
  userName: string;
  type: number;
  oldValue: string | null;
  newValue: string | null;
  changeQuantity: number | null;
  createdAt: string;
}
