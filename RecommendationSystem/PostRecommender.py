import chromadb
import torch
from chromadb.api.types import IncludeEnum
from numpy import ndarray
from Post import Post
from sentence_transformers import SentenceTransformer

class PostRecommender:
    def __init__(self, model: SentenceTransformer = SentenceTransformer('"all-MiniLM-L6-v2"'), settings=chromadb.Settings(is_persistent=True, chroma_server_http_port=7000)):
        self.client = chromadb.PersistentClient(path="./data/expert_bridge_posts.db", settings=settings)
        self.model: SentenceTransformer = model
        self.posts_collection = self.client.get_or_create_collection("expert_bridge_posts")
        self.device = "cuda" if torch.cuda.is_available() else "cpu"

    def add_post(self, post: Post) -> None:
        """
        Add a post to the database.

        Args:
            post: Post object

        Returns:
            None
        """
        self.posts_collection.add(
            documents=post.content,
            embeddings=self.get_embeddings(post.content),
            metadatas={key: value for key, value in post.to_dict().items() if key != "content"},
            ids=post.uuid
        )

    def get_embeddings(self, content: str) -> ndarray:
        """
        Get embeddings for a single post.

        Args:
            content: Post content

        Returns:
            Embedding for the post in numpy array format
        """
        return self.model.encode(
            sentences=[content],
            show_progress_bar=False,
            device=self.device
        )

    def get_embedding(self, posts: list[str]) -> ndarray:
        """
        Get embeddings for a list of posts.

        Args:
            posts: List of posts
            
        Returns:
            List of embeddings for the posts in numpy array format
        """
        return self.model.encode(
            sentences=[post for post in posts],
            show_progress_bar=False,
            device=self.device
        )

    def recommend(self, query: str, tags: list[str] = None, n_results: int = 50) -> list[Post]:
        """
        Recommend posts based on tags and semantic search.

        Args:
            query: Text query for semantic search
            tags: Optional list of tags to filter by
            n_results: Number of results to return

        Returns:
            List of Post objects
        """
        where_clause = None
        if tags and len(tags) > 0:
            where_clause = {"tags": {"$in": tags}}

        if not query and not tags:
            query = f"Posts about {tags}"

        query_embeddings = self.get_embeddings(query)

        results = self.posts_collection.query(
            query_embeddings=query_embeddings[0],
            where=where_clause,
            n_results=n_results,
            include=[IncludeEnum.metadatas, IncludeEnum.documents]
        )

        posts: list[Post] = []
        for i in range(len(results['ids'])):
            post_dict = {
                "uuid": results['ids'][i],
                "content": results['documents'][i],
                **results['metadatas'][i]  # Unpacks title, tags, language, created_at
            }
            posts.append(Post(post_dict))

        return posts