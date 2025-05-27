using System.Text;

class Transformer
{
    public static byte[] ToHuffmanBinary(Board inputBoard)
    {
        var pieceToCode = new Dictionary<string, string>
        {
            { "", "0" },           // null/빈칸
            { "pawn", "101" },     // 3비트
            { "knight", "1000" },  // 4비트  
            { "bishop", "1001" },  // 4비트
            { "rook", "110" },     // 3비트
            { "queen", "1110" },   // 4비트
            { "king", "1111" }     // 4비트
        };

        StringBuilder bitString = new StringBuilder();

        // 4x4 = 16칸 순서대로
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                string pieceType = inputBoard.Grid[x, y]?.pieceType ?? "";
                bitString.Append(pieceToCode[pieceType]);
            }
        }

        return BitStringToBytes(bitString.ToString());
    }

    public static Board FromHuffmanBinary(byte[] data)
    {
        var codeToType = new Dictionary<string, string>
    {
        { "0", "" },           // 빈칸
        { "101", "pawn" },
        { "1000", "knight" },
        { "1001", "bishop" },
        { "110", "rook" },
        { "1110", "queen" },
        { "1111", "king" }
    };

        string bitString = BytesToBitString(data);
        List<Piece> pieces = new List<Piece>();  // 기물 임시 저장
        int bitPos = 0;

        // 4x4 = 16칸 순회
        for (int pos = 0; pos < 16 && bitPos < bitString.Length; pos++)
        {
            int x = pos % 4;
            int y = pos / 4;

            // 현재 위치에서 매칭되는 코드 찾기
            string matchedCode = "";
            for (int len = 1; len <= 4 && bitPos + len <= bitString.Length; len++)
            {
                string testCode = bitString.Substring(bitPos, len);
                if (codeToType.ContainsKey(testCode))
                {
                    matchedCode = testCode;
                    break;  // 첫 번째 매칭 발견하면 중단
                }
            }

            if (matchedCode != "")  // 매칭된 코드가 있으면
            {
                string pieceType = codeToType[matchedCode];
                if (pieceType != "")  // 빈칸이 아니면
                {
                    pieces.Add(new Piece(pieceType, x, y));
                }
                bitPos += matchedCode.Length;
            }
            else
                bitPos++;
        }

        // 기물 개수를 세서 Board 생성
        Board board = new Board(pieces.Count);
        foreach (var piece in pieces)
        {
            board.RegisterPiece(piece);
        }

        return board;
    }

    private static byte[] BitStringToBytes(string bitString)
    {
        // 8의 배수로 패딩
        while (bitString.Length % 8 != 0)
            bitString += "0";

        byte[] bytes = new byte[bitString.Length / 8];

        for (int i = 0; i < bytes.Length; i++)
        {
            string byteStr = bitString.Substring(i * 8, 8);
            bytes[i] = Convert.ToByte(byteStr, 2);
        }

        return bytes;
    }

    private static string BytesToBitString(byte[] data)
    {
        StringBuilder bitString = new StringBuilder();

        foreach (byte b in data)
        {
            // 각 바이트를 8비트 2진수 문자열로 변환
            string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
            bitString.Append(binaryString);
        }

        return bitString.ToString();
    }
}