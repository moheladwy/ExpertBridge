class Post:
    def __init__(self, post: dict):
        self.title = post["title"]
        self.content = post["content"]
        self.uuid = post["uuid"]
        self.tags = post["tags"]
        self.language = post["language"]
        self.created_at = post["created_at"]

    def to_dict(self) -> dict:
        return {
            "title": self.title,
            "content": self.content,
            "uuid": self.uuid,
            "tags": self.tags,
            "language": self.language,
            "created_at": self.created_at,
        }

    def from_dict(self, post: dict) -> None:
        self.title = post["title"]
        self.content = post["content"]
        self.uuid = post["uuid"]
        self.tags = post["tags"]
        self.language = post["language"]
        self.created_at = post["created_at"]
