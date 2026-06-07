export type ProductCategory = "sofa" | "lamp" | "chair";

export type Product = {
  id: string;
  slug: string;
  name: string;
  price: string;
  image: string;
  description: string;
  category?: ProductCategory;
  badge?: string;
};

export type Room = {
  id: string;
  label: string;
  image: string;
};

export type Store = {
  id: string;
  city: string;
  address: string[];
  image: string;
};

export type MenuGroup = {
  title: string;
  links: string[];
};

export type CollageImage = {
  id: string;
  image: string;
  alt: string;
  className: string;
};

export type CartItem = {
  product: Product;
  quantity: number;
};
