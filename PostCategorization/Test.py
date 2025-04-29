import requests
import json
from OutputFormat import CategorizationResponse, TranslateTagsResponse
from pydantic import ValidationError

# BASE_URL = "https://categorizer.expertbridge.duckdns.org"
BASE_URL = "http://127.0.0.1:5000"
CATEGORIZE_ENDPOINT = BASE_URL + "/categorize"
TRANSLATE_ENDPOINT = BASE_URL + "/translate-tags"


def test_categorize_api():
    """Test the categorize API with an Arabic car problem post"""
    # Test post with proper title and content separation
    post_title = "مشكلة في العربية: صوت غريب وأداء مش طبيعي – محتاج نصائح!"
    post_content = """السلام عليكم يا جماعة،
    أنا عندي مشكلة في عربيتي ومش عارف أصلحها لوحدي. من كام يوم بقى في صوت غريب جاي من المحرك، وخصوصًا لما بضغط على البنزين أو وأنا في السكة. الصوت شكله طحن أو فرقعة مش عادية، وكمان لاحظت إن أداء العربية بقى ضعيف شوية؛ مثلاً بتاخد وقت أكتر في التسارع ومش ردة فعلها زي الأول.

    أنا تأكدت إن مستوى الزيت والسوائل تمام، ومفيش أي تسريب واضح. هل حد فيكم واجه حاجة مشابهة؟ إيه الأسباب المحتملة للصوت ده وهل ممكن يكون في مشكلة في المحرك أو علبة التروس؟ طبعا قبل ما أودّيها للميكانيكي، عايز أعرف رأيكم لو عندكم نصائح أو تجارب شخصية ممكن تفيدني.

    شكراً مقدماً على أي مساعدة أو نصيحة!"""

    # Prepare the request payload according to InputFormat structure
    payload = {
        "title": post_title,
        "content": post_content,
        "tags": []  # Optional, can be empty
    }

    # Send POST request to the API
    try:
        response = requests.post(CATEGORIZE_ENDPOINT, json=payload)

        # Check if request was successful
        if response.status_code == 200:
            # Parse the JSON response
            result = response.json()
            print("Request successful!")
            print("\nAPI Response:")
            print("```json")
            print(json.dumps(result, ensure_ascii=False, indent=2))
            print("```")

            # Validate response against schema
            try:
                validated_response = CategorizationResponse.model_validate(
                    result)
                print("\nValidation successful!")
                print(f"Language detected: {validated_response.language}")
                print(f"Number of tags: {len(validated_response.tags)}")
                print("Tags:")
                for tag in validated_response.tags:
                    print(
                        f"- English: {tag.EnglishName} | Arabic: {tag.ArabicName}")
                    print(f"  Description: {tag.Description}")
            except ValidationError as ve:
                print(f"Response validation failed: {ve}")

        else:
            print(f"Request failed with status code: {response.status_code}")
            print(f"Response content: {response.text}")

    except Exception as e:
        print(f"An error occurred: {str(e)}")


def test_translate_tags_api():
    """Test the translate-tags API with a mix of English and Arabic tags"""
    # Test tags in different languages
    tags = ["car maintenance", "محرك", "transmission", "صيانة", "engine noise"]

    # Prepare the request payload
    payload = {
        "tags": tags
    }

    # Send POST request to the API
    try:
        response = requests.post(TRANSLATE_ENDPOINT, json=payload)

        # Check if request was successful
        if response.status_code == 200:
            # Parse the JSON response
            result = response.json()
            print("Request successful!")
            print("\nTranslate Tags API Response:")
            print("```json")
            print(json.dumps(result, ensure_ascii=False, indent=2))
            print("```")

            # Validate response against schema
            try:
                validated_response = TranslateTagsResponse.model_validate(
                    result)
                print("\nValidation successful!")
                print("Translated Tags:")
                for tag in validated_response.tags:
                    print(
                        f"- English: {tag.EnglishName} | Arabic: {tag.ArabicName}")
                    print(f"  Description: {tag.Description}")
            except ValidationError as ve:
                print(f"Response validation failed: {ve}")

        else:
            print(f"Request failed with status code: {response.status_code}")
            print(f"Response content: {response.text}")

    except Exception as e:
        print(f"An error occurred: {str(e)}")


if __name__ == "__main__":
    test_categorize_api()
    print("\n" + "="*50 + "\n")
    test_translate_tags_api()
